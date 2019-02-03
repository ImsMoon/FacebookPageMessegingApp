using FacebookPageMessegingApp.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace FacebookPageMessegingApp
{
    public partial class MessageAnaliticsForm : Form
    {
        List<ConversationModel> ListOfMsg = new List<ConversationModel>();


        public MessageAnaliticsForm(List<ConversationModel> ConvList)
        {
            InitializeComponent();
            ListOfMsg = ConvList;
        }

        public void MessageAnliticsByDay()
        {
            string[] seriesArray = new string[ListOfMsg.Count];
            int[] pointsArray = new int[ListOfMsg.Count];

            ConversationModel conv = new ConversationModel();
            int count = 0;
            foreach (var time in ListOfMsg)
            {
                seriesArray[count] = time.updated_time.Day.ToString();
                pointsArray[count] = Convert.ToInt32(time.message_count);
                count++;
            }

            // Set palette.
            this.chart1.Palette = ChartColorPalette.SeaGreen;

            // Set title.
            this.chart1.Titles.Clear();
            this.chart1.Titles.Add("Message by Day");

            this.chart1.Series.Clear();
            foreach (var series in chart1.Series)
            {
                series.Points.Clear();
            }
            // Add series.
            for (int i = 0; i < seriesArray.Length; i++)
            {
                try
                {
                    // Add series.
                    Series series = this.chart1.Series.Add(seriesArray[i]);
                    // Add point.
                    series.Points.Add(pointsArray[i]);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        public void MessageAnliticsByDATE()
        {
            string[] seriesArray = new string[ListOfMsg.Count];
            int[] pointsArray = new int[ListOfMsg.Count];



            ConversationModel conv = new ConversationModel();
            int count = 0;
            foreach (var time in ListOfMsg)
            {
                seriesArray[count] = time.updated_time.DayOfWeek.ToString();
                pointsArray[count] = Convert.ToInt32(time.message_count);
                count++;
            }

            // Set palette.
            this.chart1.Palette = ChartColorPalette.SeaGreen;

            // Set title.
            this.chart1.Titles.Clear();
            this.chart1.Titles.Add("Message Amoount by Dates of Week");

            this.chart1.Series.Clear();
            foreach (var series in chart1.Series)
            {
                series.Points.Clear();
            }

            // Add series.
            for (int i = 0; i < seriesArray.Length; i++)
            {
                // Add series.
                try
                {
                    // Add series.
                    Series series = this.chart1.Series.Add(seriesArray[i]);
                    // Add point.
                    series.Points.Add(pointsArray[i]);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        public void MessageAnliticsByMonth()
        {
            string[] seriesArray = new string[ListOfMsg.Count];
            int[] pointsArray = new int[ListOfMsg.Count];

            ConversationModel conv = new ConversationModel();
            int count = 0;
            foreach (var time in ListOfMsg)
            {
                seriesArray[count] = time.updated_time.Date.ToString("MMM");
                pointsArray[count] = Convert.ToInt32(time.message_count);
                count++;
            }

            // Set palette.
            this.chart1.Palette = ChartColorPalette.SeaGreen;

            // Set title.
            this.chart1.Titles.Clear();
            this.chart1.Titles.Add("Message Amount by Months");

            this.chart1.Series.Clear();
            foreach (var series in chart1.Series)
            {
                series.Points.Clear();
            }

            // Add series.
            for (int i = 0; i < seriesArray.Length; i++)
            {

                try
                {
                    // Add series.
                    Series series = this.chart1.Series.Add(seriesArray[i]);
                    // Add point.
                    series.Points.Add(pointsArray[i]);
                }
                catch (Exception)
                {
                    continue;
                }


            }
        }

        private void MessageAnaliticsForm_Load(object sender, EventArgs e)
        {
            List<string> reportlist = new List<string>();
            ChartReportList.SelectedItem = null;
            ChartReportList.SelectedItem = "Select";
            reportlist.Add("Message By Day");
            reportlist.Add("Message By Days of week");
            reportlist.Add("Message By Months");
            ChartReportList.DataSource = reportlist;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png|JPeg Image|*.jpg";
            saveFileDialog.Title = "Save Chart As Image File";
            saveFileDialog.FileName = "Sample.png";

            DialogResult result = saveFileDialog.ShowDialog();
            saveFileDialog.RestoreDirectory = true;

            if (result == DialogResult.OK && saveFileDialog.FileName != "")
            {
                try
                {
                    if (saveFileDialog.CheckPathExists)
                    {
                        if (saveFileDialog.FilterIndex == 2)
                        {
                            chart1.SaveImage(saveFileDialog.FileName, ChartImageFormat.Jpeg);
                        }
                        else if (saveFileDialog.FilterIndex == 1)
                        {
                            chart1.SaveImage(saveFileDialog.FileName, ChartImageFormat.Png);
                        }

                    }
                    else
                    {
                        MessageBox.Show("Given Path does not exist");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ChartReportList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ChartReportList.SelectedItem.ToString() == "Message By Day")
                MessageAnliticsByDay();
            else if (ChartReportList.SelectedItem.ToString() == "Message By Days of week")
                MessageAnliticsByDATE();
            else if (ChartReportList.SelectedItem.ToString() == "")
            {
                this.chart1.Series.Clear();
                foreach (var series in chart1.Series)
                {
                    series.Points.Clear();
                }
            }
            else
                MessageAnliticsByMonth();
        }
    }
}
