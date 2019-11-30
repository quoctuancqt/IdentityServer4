namespace IdentityServer.Dtos.Base
{
    public class QuerySelectListItemDefault
    {
        public virtual string SearchKey { get; set; }

        public virtual string SelectedValue { get; set; }

        public virtual string SelectedDefaultValue { get; set; }

        public virtual int GetSkip()
        {
            return 0;
        }

        public virtual int GetTake()
        {
            return 1000;
        }
    }
}
