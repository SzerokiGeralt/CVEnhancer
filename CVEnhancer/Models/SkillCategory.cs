namespace CVEnhancer.Models
{
    public class SkillCategory
    {
        public int SkillCategoryId { get; set; }
        public string Name { get; set; }
        public List<Skill> Skills { get; set; } = new ();
    }
}
