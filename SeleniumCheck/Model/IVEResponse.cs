namespace SeleniumCheck.Model
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class IVEResponse
    {

        private int statusField;

        private string errorStringField;

        private string aPPLaunchTokenField;

        /// <remarks/>
        public int Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public string ErrorString
        {
            get
            {
                return this.errorStringField;
            }
            set
            {
                this.errorStringField = value;
            }
        }

        /// <remarks/>
        public string APPLaunchToken
        {
            get
            {
                return this.aPPLaunchTokenField;
            }
            set
            {
                this.aPPLaunchTokenField = value;
            }
        }
    }
}
