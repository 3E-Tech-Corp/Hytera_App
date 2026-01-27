using YYTools;

namespace FunTimePIE
{
    public class FTDB
    {
        private readonly IConfiguration _configuration;
        public string ConStr
        {
            get { return _ConStr; }
            set { _ConStr = value; }
        }
        private string _ConStr="";

        public DBTool MyDBT;

        public FTDB(IConfiguration configuration)
        {
            _configuration = configuration;
            _ConStr=_configuration.GetConnectionString("DefaultConnection");
      
            MyDBT = new DBTool(_ConStr);
        }
    }
}
