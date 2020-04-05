namespace MasterRad.DTO.RQ
{
    public class SearchStudentsRQ
    {
        public SearchStudentsRQ(int pageSize)
        {
            PageSize = pageSize;
        }

        public int PageSize { get; set; }
        public string FirstNameStartsWith { get; set; }
        public string LastNameStartsWith { get; set; }
        public string EmailStartsWith { get; set; }
    }   
}
