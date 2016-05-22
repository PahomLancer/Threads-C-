using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace threads
{
    public partial class Form1 : Form
    {
        private List<WorkThread> workThreads = new List<WorkThread>();
        private int result;

        private delegate void formShowResult();
        private delegate void formUpdateProgress(int progr);


        private void formShowResultImpl()
        {
            this.textBox1.Text = "" + this.result;
        }

        private void formUpdateProgressImpl(int i)
        {
            this.progressBar1.Value+=i;
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.result = 0;
            this.workThreads.Clear();

            for (int i = 1; i <= 4; i++)
            {
                WorkThread threadImpl = new WorkThread(new Action<int>(this.workThreadCallback), new Action<WorkThread>(this.workThreadSuccessCallback));
                threadImpl.thread.Start();

                this.workThreads.Add(threadImpl);
            }
        }


        private void workThreadCallback(int input)
        {
            formUpdateProgress progress = new formUpdateProgress(this.formUpdateProgressImpl);
            this.Invoke(progress, new object[] { input });
        }

        private void workThreadSuccessCallback(WorkThread input)
        {
            this.workThreads.Remove(input);

            this.result += input.result;

            if (this.workThreads.Count == 0)
            {
                
                formShowResult method = new formShowResult(this.formShowResultImpl);
                this.Invoke(method);
            }
        }

        
        class WorkThread
        {
            public Thread thread { get; set; }
            public int result { get; set; }
            private Action<int> progressCallback;
            private Action<WorkThread> successCallback;
            
            public WorkThread(Action<int> progressCallback, Action<WorkThread> successCallback)
            {
                this.progressCallback = progressCallback;
                this.successCallback = successCallback;
                this.thread = new Thread(proc);
            }

            private void proc()
            {
                int sum = 0;

                for (int i = 0; i < 1000; i++)
                {
                    sum += i+1;

                    if (i % 20 == 0) 
                    {
                        this.progressCallback.Invoke(20);
                    }

                    Thread.Sleep(20);
                }

                this.result = sum;
                this.successCallback.Invoke(this);
            }
        
        
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            foreach (WorkThread workThread in this.workThreads)
            {
                workThread.thread.Suspend();
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            foreach (WorkThread workThread in this.workThreads)
            {
                workThread.thread.Resume();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (WorkThread workThread in this.workThreads)
            {
                workThread.thread.Abort();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }

     
}


