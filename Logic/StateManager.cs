using System.Collections.Generic;
using System.Linq;

namespace RARPEditor.Logic
{
    /// <summary>
    /// A generic class to manage undo and redo history for an object state.
    /// </summary>
    /// <typeparam name="T">The type of the state object being managed.</typeparam>
    public class StateManager<T> where T : class
    {
        private readonly Stack<T> _undoStack = new();
        private readonly Stack<T> _redoStack = new();

        public bool CanUndo => _undoStack.Count > 1;
        public bool CanRedo => _redoStack.Any();

        public StateManager(T initialState)
        {
            RecordState(initialState);
        }

        /// <summary>
        /// Records a new state, pushing it onto the undo stack and clearing the redo stack.
        /// </summary>
        public void RecordState(T state)
        {
            _undoStack.Push(state);
            _redoStack.Clear();
        }

        /// <summary>
        /// Moves the current state to the redo stack and returns the previous state from the undo stack.
        /// </summary>
        /// <returns>The previous state, or null if undo is not possible.</returns>
        public T? Undo()
        {
            if (!CanUndo) return null;

            var currentState = _undoStack.Pop();
            _redoStack.Push(currentState);
            return _undoStack.Peek();
        }

        /// <summary>
        /// Moves a state from the redo stack to the undo stack and returns it.
        /// </summary>
        /// <returns>The redone state, or null if redo is not possible.</returns>
        public T? Redo()
        {
            if (!CanRedo) return null;

            var futureState = _redoStack.Pop();
            _undoStack.Push(futureState);
            return futureState;
        }

        /// <summary>
        /// Clears all history.
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}