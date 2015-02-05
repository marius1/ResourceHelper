using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ResourceHelper
{
    public partial class Main : Form
    {
        private Dictionary<string, TranslationNode> keys = new Dictionary<string, TranslationNode>();
        private List<string> show = new List<string>();

        public Main()
        {
            InitializeComponent();

            txtFilter.TextChanged += txtFilter_TextChanged;

            var resourceFiles = new List<string>()
            {
                @"C:\Repos\central\4_PresentatieLaag\Controls\Translation\GlobalResources.en.resx",
                @"C:\Repos\central\4_PresentatieLaag\Controls\Translation\GlobalResources.resx",
                @"C:\Repos\central\4_PresentatieLaag\Funda.Web.Common.Resources\Strings.en.resx",
                @"C:\Repos\central\4_PresentatieLaag\Funda.Web.Common.Resources\Strings.resx"
            };
            
            foreach (var file in resourceFiles)
            {
                var doc = new XmlDocument();
                doc.Load(file);

                foreach (XmlNode node in doc.SelectNodes("//root/data"))
                {
                    if (node.Attributes == null || node.Attributes["name"] == null) 
                        continue;

                    string name = node.Attributes["name"].InnerText;
                    var value = node.SelectSingleNode("value");
                    if (value == null)
                        continue;

                    if (keys.ContainsKey(name))
                    {
                        keys[name].Translations.Add(value.InnerText);
                    }
                    else
                    {
                        keys.Add(name, new TranslationNode()
                        {
                            Key = name, 
                            Translations = new List<string>()
                            {
                                value.InnerText
                            }
                        });
                    }
                }
            }

            show.AddRange(keys.Keys);

            lstKeys.Items.AddRange(show.ToArray());
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            show.Clear();

            foreach (var key in keys)
            {
                if (key.Key.Contains(txtFilter.Text))
                    show.Add(key.Key);
            }

            lstKeys.Items.Clear();
            lstKeys.Items.AddRange(show.ToArray());

            if (show.Count > 0)
            {
                lstKeys.SelectedIndex = 0;
            }
        }

        private void lstKeys_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lstKeys.SelectedIndex;
            if (index >= 0)
            {
                var key = lstKeys.Items[index].ToString();
                if (keys.ContainsKey(key))
                {
                    var output = "";
                    foreach (var translation in keys[key].Translations)
                    {
                        output += translation + Environment.NewLine;
                    }
                    txtOutput.Text = output;
                }
            }
        }
    }

    public class TranslationNode
    {
        public List<string> Translations { get; set; }
        public string Key { get; set; }
    }
}
