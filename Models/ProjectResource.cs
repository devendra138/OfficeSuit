namespace OfficeSuit.Models
{
    public class ProjectResource
    {
        public int Id { get; set; }
        public int? UserId { get; set; }

        public string UserName { get; set; }

        public int DesignationId { get; set; }
        public string DesignationName { get; set; }

        public int ProjectId { get; set; }
    }
}
