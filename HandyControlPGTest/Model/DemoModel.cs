using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using HandyControl.Controls;
using HandyControlPGTest.Editors;

namespace HandyControlPGTest.Model
{
    [Editor(typeof(PropertyGridEditor), typeof(PropertyEditorBase))]
    public class DemoModel
    {
        public DemoModel()
        {
            List = new List<string>();
            dList = new List<DemoModel>();
        }

        public string String { get; set; }
        public Gender Enum { get; set; }
        public int Integer { get; set; }
        [Editor(typeof(CollectionEditor), typeof(PropertyEditorBase))]
        public List<string> List { get; set; }
        [Editor(typeof(CollectionEditor), typeof(PropertyEditorBase))]
        public List<DemoModel> dList { get; set; }

        public DemoModel DdModel { get; set; }

        public override string ToString() => String;
    }


    public class PropertyGridDemoModel
    {

        public PropertyGridDemoModel()
        {
            List = new List<string>() { "aaa", "bbb" };
            List2 = new List<DemoModel>();

            DemoModel = new DemoModel()
            {
                Enum = Gender.Female,
                String = "xxxx",
                Integer = 790
            };
        }
        public DemoModel DemoModel { get; set; }

        [Editor(typeof(CollectionEditor), typeof(PropertyEditorBase))]
        public List<string> List { get; set; }

        [Editor(typeof(CollectionEditor), typeof(PropertyEditorBase))]
        public List<DemoModel> List2 { get; set; }

        public string String { get; set; }

        public int Integer { get; set; }

        public bool Boolean { get; set; }

        public Gender Enum { get; set; }
        public ImageSource ImageSource { get; set; }
    }


    public enum Gender
    {
        Male,
        Female
    }
}
