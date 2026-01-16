namespace CVEnhancer.Models
{
    public class SkillAlias
    {
        public int SkillAliasId { get; set; }
        public string Alias { get; set; }
        public Skill Skill { get; set; }
    }
}
