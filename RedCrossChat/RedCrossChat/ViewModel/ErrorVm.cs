namespace RedCrossChat
{
    public class ErrorVm
    {
        public int code { get; set; } = 200;

        public string message { get; set; }

        public string getBannerMessage()
        {
            switch (code)
            {
                case 401:

                    return code.ToString() + " Access Denied";

                default:
                    return code.ToString() + "Page Broken";
            }
        }
    }
}
