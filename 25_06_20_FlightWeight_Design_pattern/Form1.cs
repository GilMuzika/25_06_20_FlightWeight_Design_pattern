using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _25_06_20_FlightWeight_Design_pattern
{
    public partial class Form1 : Form
    {
        private Random _rnd = new Random();
        private Timer _timer = new Timer();

        private RectanglesFactory _rectanglesFactory;

        private ConcurrentQueue<PictureBox> _pictureBoxPool = new ConcurrentQueue<PictureBox>();        
        private ConcurrentQueue<WeakReference<MyRectangle>> _repository = new ConcurrentQueue<WeakReference<MyRectangle>>();
        

        public Form1()
        {            
            InitializeComponent();
            pnlMainView.drawBorder(4, Color.Black);
            FillRepository();
            Initialize();

        }

        private void Initialize()
        {
            _rectanglesFactory = new RectanglesFactory(_pictureBoxPool, _repository, this.pnlMainView.Width, this.pnlMainView.Height);

            this.Text = "Flyweight design pattern example";

            lblInfo.drawBorder(10, Color.DarkBlue);
            lblInfo.Padding = new Padding(10);
            

            lblInfo.Visible = true;
            btnDisplay.Click += (object sender, EventArgs e) => 
            {
                lblInfo.Visible = false;
                _timer.Interval = 10;
                _timer.Tick += (object sender2, EventArgs e2) => 
                {
                    _rectanglesFactory.RectangleFactory();
                };
                _timer.Start();

                


            };
        }

        private async void FillRepository()
        {
            
            await FillRepositoryInternal();
            
        }

        private async Task FillRepositoryInternal()
        {
            Task tsk = Task.Run(async() => 
            {
                for (int i = 0; i < 100000; i++)
                {
                    await Task.Run(() => 
                    {
                        MyRectangle rectangle = new MyRectangle(Statics.RandomColor(), new Point(Statics.generateRandomNumberBetween(4, this.pnlMainView.Width/4), Statics.generateRandomNumberBetween(4, this.pnlMainView.Height/4)), Statics.generateRandomNumberBetween(10, this.pnlMainView.Width), Statics.generateRandomNumberBetween(10, this.pnlMainView.Height));
                        _repository.Enqueue(new WeakReference<MyRectangle>(rectangle));

                        string jrect = $"Rectangle N# {i+1} added, \n {JsonConvert.SerializeObject(rectangle)}"; 

                        Action a = () => { lblInfo.Text = jrect; };
                        if (lblInfo.InvokeRequired) 
                        {
                            try
                            {
                                lblInfo.BeginInvoke(a);
                            }
                            catch(Exception ex)
                            {
                                MessageBox.Show($"{ex.GetType().Name}\n\n{ex.Message}");
                            }
                        }
                        else a();



                    });
                }
            });
            await tsk;
            if(tsk.IsCompleted) await FillpictureBoxPool();

        }

        private async Task FillpictureBoxPool()
        {
            Task tsk = Task.Run(() => 
            {
                Action a = () => { lblInfo.Text += "\n----------------------\n"; };
                if (lblInfo.InvokeRequired) lblInfo.BeginInvoke(a);
                else a();
                

                for (int i = 0; i < 20; i++)
                {
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Visible = false;
                    pictureBox.Name = (i+1).ToString();

                    a  = () => 
                    {
                        this.pnlMainView.Controls.Add(pictureBox); 
                        lblInfo.Text += $"PictureBox N# {pictureBox.Name} added\n";
                    };
                    if (this.InvokeRequired || this.pnlMainView.InvokeRequired || this.lblInfo.InvokeRequired) this.BeginInvoke(a);
                    else a();
                    _pictureBoxPool.Enqueue(pictureBox);                    
                }
            });
            await tsk;
            if (tsk.IsCompleted) 
            {
                btnDisplay.Enabled = true;                

            }

        }


    }
}
