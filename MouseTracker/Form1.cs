using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using System.Threading;
using System.IO;

using CsvHelper;

namespace MouseTracker
{
    public partial class Form1 : Form
    {
        private Thread record;
        private List<Coords> data = new List<Coords>();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox1.Text))
            {
                startRecord();
            }
            else
            {
                MessageBox.Show("Укажите корректный путь до папки", "Mouse Tracker");
            }
        }

        private void startRecord()
        {
            startButtonActive(false);
            data.Clear();

            record = new Thread(new ThreadStart(processRecord));
            record.Start();
        }

        private void processRecord()
        {
            while (true)
            {
                data.Add(new Coords(Cursor.Position.X, Cursor.Position.Y, getTimeStampDouble()));
                Thread.Sleep(1000 / (int) numericUpDown1.Value);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                record.Abort();
                saveData(Path.Combine(textBox1.Text, getTimeStampInt().ToString() + ".csv"));
            }
            catch
            {
                MessageBox.Show("Ой, что-то пошло не так(", "Mouse Tracker");
            }

            startButtonActive(true);
        }

        private void startButtonActive(bool mode)
        {
            button1.Enabled = mode;
            button2.Enabled = !button1.Enabled;
        }

        private void saveData(string path)
        {
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(data);
            }
        }

        private int getTimeStampInt()
        {
            return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
        }

        private double getTimeStampDouble()
        {
            return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public struct Coords
        {
            public int x { get; set; }
            public int y { get; set; }
            public double timestamp { get; set; }

            public Coords(int x, int y, double timestamp)
            {
                this.x = x;
                this.y = y;
                this.timestamp = timestamp;
            }
        }
    }
}
