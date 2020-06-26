using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _25_06_20_FlightWeight_Design_pattern
{
    internal class RectanglesFactory
    {
        private ConcurrentQueue<PictureBox> _pictureBoxPool;
        private ConcurrentQueue<WeakReference<MyRectangle>> _repository;
        private int _rectMaxWidth;
        private int _rectMaxHeight;

        public RectanglesFactory(ConcurrentQueue<PictureBox> pictureBoxPool, ConcurrentQueue<WeakReference<MyRectangle>> repository, int rectMaxWidth, int rectMaxHeight)
        {
            _pictureBoxPool = pictureBoxPool;
            _repository = repository;
            this._rectMaxWidth = rectMaxWidth;
            this._rectMaxHeight = rectMaxHeight;
        }

        public void RectangleFactory()
        {
            bool istaken = false;
            WeakReference<MyRectangle> weakRectangle = null;
            do {
                istaken = _repository.TryDequeue(out weakRectangle);
            } while (!istaken);

            istaken = weakRectangle.TryGetTarget(out MyRectangle rectangle);
            if (!istaken)
            {                
                rectangle = new MyRectangle(Statics.RandomColor(), new Point(Statics.generateRandomNumberBetween(4, _rectMaxWidth/4), Statics.generateRandomNumberBetween(4, _rectMaxWidth/4)), Statics.generateRandomNumberBetween(10, _rectMaxWidth), Statics.generateRandomNumberBetween(10, _rectMaxHeight));
            }


            PictureBox concreteRectangle = null;
            istaken = false;
            do
            {
                istaken = _pictureBoxPool.TryDequeue(out concreteRectangle);
            }
            while (!istaken);
            concreteRectangle.Location = rectangle.Location;
            concreteRectangle.Width = rectangle.Width;
            concreteRectangle.Height = rectangle.Height;

            Bitmap rectangleImage = new Bitmap(rectangle.Width, rectangle.Height);            
            Graphics graphics = Graphics.FromImage(rectangleImage);
            graphics.FillRectangle(new SolidBrush(rectangle.Color), new RectangleF(new Point(1, 1), new Size(rectangle.Width-2, rectangle.Height-2)));
            graphics.Dispose();

            concreteRectangle.drawBorder(1, Color.Black);
            concreteRectangle.Image = rectangleImage;            

            concreteRectangle.Visible = true;


            _pictureBoxPool.Enqueue(concreteRectangle);
            weakRectangle.SetTarget(rectangle);
            _repository.Enqueue(weakRectangle);
        }
    }
}
