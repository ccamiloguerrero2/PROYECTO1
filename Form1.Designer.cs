using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RecordLookupApp
{
    public partial class MainForm : Form
    {
        private List<string> names;
        private List<string> filteredNames;

        public MainForm()
        {
            InitializeComponent();
            InitializeNames();
        }

        private void InitializeNames()
        {
            string url = "https://10.0.0.6/api/v2/devices/statistics?fields=status,name,tower,mode,ip,ip_wan,lan_status,lan_speed_status&limit=0";
            string token = "ad16fd38497edd93e41c2203bcc21c9960d08bed";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                HttpResponseMessage response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseData);
                    var data = jsonData.data;
                    names = new List<string>();
                    foreach (var item in data)
                    {
                        names.Add((string)item.name);
                    }
                    combobox.Items.AddRange(names.ToArray());
                }
            }
        }

        private void LookupRecords()
        {
            string keyword = searchEntry.Text;
            filteredNames = names.Where(name => name.ToLower().Contains(keyword.ToLower())).ToList();
            combobox.Items.Clear();
            combobox.Items.AddRange(filteredNames.ToArray());
        }

        private void SearchRecords()
        {
            string selectedName = combobox.SelectedItem as string;
            if (selectedName != null)
            {
                resultLabel.Text = $"Selected: {selectedName}";
            }
            else
            {
                resultLabel.Text = "Not found.";
            }
        }

        private void ClearRecords()
        {
            combobox.Text = "";
            resultLabel.Text = "";
            combobox.Items.Clear();
            combobox.Items.AddRange(names.ToArray());
        }

        private void LookupButton_Click(object sender, EventArgs e)
        {
            LookupRecords();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            SearchRecords();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearRecords();
        }
    }
}
