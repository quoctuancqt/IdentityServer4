namespace IdentityServer.Dtos.Base
{
    public class QuerySearchDefault
    {
        public virtual string SearchKey { get; set; }

        public virtual int Page { get; set; } = 1;

        public virtual int Size { get; set; } = 10;

        public virtual int GetSkip()
        {
            return (Page - 1) * Size;
        }

        public virtual int GetTake()
        {
            return Size;
        }
    }
}
