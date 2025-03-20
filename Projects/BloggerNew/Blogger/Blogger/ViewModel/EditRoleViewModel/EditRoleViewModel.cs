namespace Blogger.ViewModel.EditRoleViewModel
{
    public class EditRoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> UserNames { get; set; } = new List<string>();
    }
}
