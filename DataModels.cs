#nullable enable
namespace RARPEditor.Models
{
    // Represents a single part of a condition, like a memory address or a constant value.
    public class Operand
    {
        public string Type { get; set; } = "";
        public string Size { get; set; } = "";
        public string Value { get; set; } = "";

        // Add a clone method for deep copying.
        public Operand Clone() => new Operand
        {
            Type = this.Type,
            Size = this.Size,
            Value = this.Value
        };
    }

    // Represents a full condition line (e.g., Mem[0x1234] == 10).
    public class AchievementCondition
    {
        public int ID { get; set; }
        public string Flag { get; set; } = "";
        public Operand LeftOperand { get; set; } = new Operand();
        public string Operator { get; set; } = "";
        public Operand RightOperand { get; set; } = new Operand();
        public uint RequiredHits { get; set; }

        // Fix: Ensure the clone method creates new Operand instances for a true deep copy.
        // Add a clone method for deep copying the condition and its operands.
        public AchievementCondition Clone()
        {
            return new AchievementCondition
            {
                ID = this.ID,
                Flag = this.Flag,
                LeftOperand = this.LeftOperand.Clone(),
                Operator = this.Operator,
                RightOperand = this.RightOperand.Clone(),
                RequiredHits = this.RequiredHits
            };
        }
    }

    // A collection of conditions, representing either a Core or an Alt group.
    public class AchievementConditionGroup
    {
        public string GroupName { get; set; } = "";
        public List<AchievementCondition> Conditions { get; set; } = new List<AchievementCondition>();
    }
}
#nullable disable