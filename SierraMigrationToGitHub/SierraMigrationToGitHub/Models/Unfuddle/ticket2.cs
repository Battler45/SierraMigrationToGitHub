using System;
using System.Collections.Generic;
using System.Text;

namespace SierraMigrationToGitHub.Models.lol.Unfuddle
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class ticket
    {

        private ticketAssigneeid assigneeidField;

        private ticketComponentid componentidField;

        private ticketCreatedat createdatField;

        private object descriptionField;

        private string descriptionformatField;

        private object descriptionformattedField;

        private ticketDueon dueonField;

        private object dueonformattedField;

        private ticketField1valueid field1valueidField;

        private ticketField2valueid field2valueidField;

        private ticketField3valueid field3valueidField;

        private ticketHoursestimatecurrent hoursestimatecurrentField;

        private ticketHoursestimateinitial hoursestimateinitialField;

        private ticketID idField;

        private ticketMilestoneid milestoneidField;

        private ticketNumber numberField;

        private string priorityField;

        private ticketProjectid projectidField;

        private ticketReporterid reporteridField;

        private string resolutionField;

        private object resolutiondescriptionField;

        private string resolutiondescriptionformatField;

        private object resolutiondescriptionformattedField;

        private ticketSeverityid severityidField;

        private string statusField;

        private object summaryField;

        private ticketUpdatedat updatedatField;

        private ticketVersionid versionidField;

        private object field1valueField;

        private object field2valueField;

        private object field3valueField;

        private ticketComments commentsField;

        private ticketAttachments attachmentsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("assignee-id")]
        public ticketAssigneeid assigneeid
        {
            get
            {
                return this.assigneeidField;
            }
            set
            {
                this.assigneeidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("component-id")]
        public ticketComponentid componentid
        {
            get
            {
                return this.componentidField;
            }
            set
            {
                this.componentidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("created-at")]
        public ticketCreatedat createdat
        {
            get
            {
                return this.createdatField;
            }
            set
            {
                this.createdatField = value;
            }
        }

        /// <remarks/>
        public object description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description-format")]
        public string descriptionformat
        {
            get
            {
                return this.descriptionformatField;
            }
            set
            {
                this.descriptionformatField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("description-formatted")]
        public object descriptionformatted
        {
            get
            {
                return this.descriptionformattedField;
            }
            set
            {
                this.descriptionformattedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("due-on")]
        public ticketDueon dueon
        {
            get
            {
                return this.dueonField;
            }
            set
            {
                this.dueonField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("due-on-formatted")]
        public object dueonformatted
        {
            get
            {
                return this.dueonformattedField;
            }
            set
            {
                this.dueonformattedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("field1-value-id")]
        public ticketField1valueid field1valueid
        {
            get
            {
                return this.field1valueidField;
            }
            set
            {
                this.field1valueidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("field2-value-id")]
        public ticketField2valueid field2valueid
        {
            get
            {
                return this.field2valueidField;
            }
            set
            {
                this.field2valueidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("field3-value-id")]
        public ticketField3valueid field3valueid
        {
            get
            {
                return this.field3valueidField;
            }
            set
            {
                this.field3valueidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("hours-estimate-current")]
        public ticketHoursestimatecurrent hoursestimatecurrent
        {
            get
            {
                return this.hoursestimatecurrentField;
            }
            set
            {
                this.hoursestimatecurrentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("hours-estimate-initial")]
        public ticketHoursestimateinitial hoursestimateinitial
        {
            get
            {
                return this.hoursestimateinitialField;
            }
            set
            {
                this.hoursestimateinitialField = value;
            }
        }

        /// <remarks/>
        public ticketID id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("milestone-id")]
        public ticketMilestoneid milestoneid
        {
            get
            {
                return this.milestoneidField;
            }
            set
            {
                this.milestoneidField = value;
            }
        }

        /// <remarks/>
        public ticketNumber number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }

        /// <remarks/>
        public string priority
        {
            get
            {
                return this.priorityField;
            }
            set
            {
                this.priorityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("project-id")]
        public ticketProjectid projectid
        {
            get
            {
                return this.projectidField;
            }
            set
            {
                this.projectidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("reporter-id")]
        public ticketReporterid reporterid
        {
            get
            {
                return this.reporteridField;
            }
            set
            {
                this.reporteridField = value;
            }
        }

        /// <remarks/>
        public string resolution
        {
            get
            {
                return this.resolutionField;
            }
            set
            {
                this.resolutionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("resolution-description")]
        public object resolutiondescription
        {
            get
            {
                return this.resolutiondescriptionField;
            }
            set
            {
                this.resolutiondescriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("resolution-description-format")]
        public string resolutiondescriptionformat
        {
            get
            {
                return this.resolutiondescriptionformatField;
            }
            set
            {
                this.resolutiondescriptionformatField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("resolution-description-formatted")]
        public object resolutiondescriptionformatted
        {
            get
            {
                return this.resolutiondescriptionformattedField;
            }
            set
            {
                this.resolutiondescriptionformattedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("severity-id")]
        public ticketSeverityid severityid
        {
            get
            {
                return this.severityidField;
            }
            set
            {
                this.severityidField = value;
            }
        }

        /// <remarks/>
        public string status
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
        public object summary
        {
            get
            {
                return this.summaryField;
            }
            set
            {
                this.summaryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("updated-at")]
        public ticketUpdatedat updatedat
        {
            get
            {
                return this.updatedatField;
            }
            set
            {
                this.updatedatField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("version-id")]
        public ticketVersionid versionid
        {
            get
            {
                return this.versionidField;
            }
            set
            {
                this.versionidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("field1-value")]
        public object field1value
        {
            get
            {
                return this.field1valueField;
            }
            set
            {
                this.field1valueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("field2-value")]
        public object field2value
        {
            get
            {
                return this.field2valueField;
            }
            set
            {
                this.field2valueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("field3-value")]
        public object field3value
        {
            get
            {
                return this.field3valueField;
            }
            set
            {
                this.field3valueField = value;
            }
        }

        /// <remarks/>
        public ticketComments comments
        {
            get
            {
                return this.commentsField;
            }
            set
            {
                this.commentsField = value;
            }
        }

        /// <remarks/>
        public ticketAttachments attachments
        {
            get
            {
                return this.attachmentsField;
            }
            set
            {
                this.attachmentsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketAssigneeid
    {

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketComponentid
    {

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketCreatedat
    {

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketDueon
    {

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketField1valueid
    {

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketField2valueid
    {

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketField3valueid
    {

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketHoursestimatecurrent
    {

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketHoursestimateinitial
    {

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketID
    {

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketMilestoneid
    {

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketNumber
    {

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketProjectid
    {

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketReporterid
    {

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketSeverityid
    {

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketUpdatedat
    {

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketVersionid
    {

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketComments
    {

        private object commentField;

        private string[] textField;

        private string typeField;

        /// <remarks/>
        public object comment
        {
            get
            {
                return this.commentField;
            }
            set
            {
                this.commentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ticketAttachments
    {

        private string attachmentField;

        private string[] textField;

        private string typeField;

        /// <remarks/>
        public string attachment
        {
            get
            {
                return this.attachmentField;
            }
            set
            {
                this.attachmentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }


}
