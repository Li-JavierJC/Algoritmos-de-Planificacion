using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Planificacion
{
    public partial class Planificacion : Form
    {
        Trabajo proceso;
        int contadorProcesos = 0;
        List<Trabajo> listaProcesos = new List<Trabajo>();
        List<Trabajo> copiaListaProcesos = new List<Trabajo>();

        List<Trabajo> listaSJF = new List<Trabajo>();
        List<Trabajo> listaPrioridades = new List<Trabajo>();
        PictureBox pb;
        Label label;

        Label labelTitulo;

        TextBox tb_tiempo;
        List<Color> listaColores = new List<Color>();
        List<Color> listaGrises = new List<Color>();

        Color miColor;

        List<int> retornoFCFS = new List<int>();
        List<int> retornoSJF = new List<int>();
        List<int> retornoRR = new List<int>();
        List<int> retornoPrioridades = new List<int>();

        List<int> esperaFCFS = new List<int>();
        List<int> esperaSJF = new List<int>();
        List<int> esperaRR = new List<int>();
        List<int> esperaPrioridades = new List<int>();

        public Planificacion()
        {
            InitializeComponent();
            listaColores.Add(Color.DarkKhaki);
            listaColores.Add(Color.DarkGreen);
            listaColores.Add(Color.DarkGoldenrod);
            listaColores.Add(Color.DarkOrange);
            listaColores.Add(Color.DarkViolet);
            listaColores.Add(Color.Brown);
            listaColores.Add(Color.Chocolate);
            listaColores.Add(Color.Azure);
            listaColores.Add(Color.Beige);
            listaColores.Add(Color.DimGray);
            listaColores.Add(Color.LightCyan);
            listaColores.Add(Color.Azure);
            listaGrises.Add(Color.LightGray);
            listaColores.Add(Color.Azure);
            listaColores.Add(Color.DarkGreen);
            listaColores.Add(Color.DarkOrange);
            listaColores.Add(Color.DarkViolet);
            listaColores.Add(Color.Brown);
            listaColores.Add(Color.Chocolate);
            listaColores.Add(Color.ForestGreen);
            listaColores.Add(Color.LightSkyBlue);
            listaColores.Add(Color.OrangeRed);
            listaColores.Add(Color.PaleGoldenrod);

        }
        private void button1_Click(object sender, EventArgs e)
        {
            int totalProcesos = Int32.Parse(textBox12.Text);

            for(int i=1; i<=totalProcesos; i++)
            {
                Registrar_trabajo solicitaProceso = new Registrar_trabajo(i);
                solicitaProceso.pasado += new Registrar_trabajo.pasar(creaProceso);
                solicitaProceso.ShowDialog(this);

                if(i == totalProcesos)
                {
                    Seleccion algoritmo = new Seleccion();
                    algoritmo.pasado += new Seleccion.pasar(ejecuta);
                    algoritmo.Show(this);
                }
            }          
        }

        public void creaProceso(int tiempo, int prioridad)
        {
            proceso = new Trabajo();
            contadorProcesos++;

            proceso.numeroProceso = contadorProcesos;
            proceso.tiempoCPU = tiempo;
            proceso.prioridad = prioridad;

            listaProcesos.Add(proceso);
        }

        public void ejecuta(List<int> listaOpcines)
        {
            foreach(int l in listaOpcines)
            {
               if(l == 0)
                {
                    ganttFCFS();
                }
               if(l == 1)
                {
                    ganttSJF();
                }
               if(l == 2)
                {
                    ganttRR();
                }
               if(l == 3)
                {
                    ganttPrioridades();
                }
            }
            poblarTabla(listaOpcines);
            tablaEspera(listaOpcines);    
        }

        public void ganttFCFS()
        {
            int tiempoTotal = 0;
            int contador = 0;
            int sizeAnterior = 0;
            int tiempoActual = 0;

            int c = 0;

            Font myFont = new Font("Arial", 16);
            labelTitulo = new Label();
            labelTitulo.Text = "FCFS (First Come First Serve)";
            labelTitulo.Font = myFont;
            labelTitulo.Location = new System.Drawing.Point(20, 20);
            labelTitulo.AutoSize = true;

            this.tabPage2.Controls.Add(labelTitulo);
            foreach (Trabajo p in listaProcesos)
            {
                tiempoTotal += p.tiempoCPU;
            }
            foreach (Trabajo p in listaProcesos)
            {
                tiempoActual += p.tiempoCPU;

                //Crea 2 PictureBox (bloque de Gantt), para el proceso y para el tiempo
                miColor = listaColores.ElementAt(contador);

                pb = new PictureBox();
                this.pb.Size = new System.Drawing.Size(((p.tiempoCPU * 1124) / tiempoTotal), 30);
                this.pb.BackColor = miColor;
                this.pb.Paint += new PaintEventHandler((sender, e) =>
                {
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    e.Graphics.DrawString(p.numeroProceso.ToString(), myFont, Brushes.Black, (((p.tiempoCPU * 1124) / tiempoTotal) / 2), 3);
                });

                tb_tiempo = new TextBox();
                this.tb_tiempo.Size = new System.Drawing.Size(((p.tiempoCPU * 1124) / tiempoTotal), 30);
                this.tb_tiempo.BackColor = Color.LightGray;
                this.tb_tiempo.TextAlign = HorizontalAlignment.Center;
                this.tb_tiempo.Font = myFont;
                this.tb_tiempo.ForeColor = Color.Gray;

                /* <<< Finaliza creación de PictureBox <<< */

                if (contador == 0) // Si es el primer proceso su posición es fija (bloque de diagrama de Gantt)
                {
                    this.pb.Location = new System.Drawing.Point(20, 50);
                    sizeAnterior = ((p.tiempoCPU * 1124) / tiempoTotal);

                    this.tb_tiempo.Location = new System.Drawing.Point(20, 80);
                    this.tb_tiempo.Text = tiempoActual.ToString();
                    retornoFCFS.Add(tiempoActual);
                    esperaFCFS.Add(tiempoActual - p.tiempoCPU);
                    //MessageBox.Show(tiempoActual.ToString() + " " + p.tiempoCPU);

                    this.tabPage2.Controls.Add(pb);
                    this.tabPage2.Controls.Add(tb_tiempo);
                }
                else
                {
                    this.pb.Location = new System.Drawing.Point(20 + sizeAnterior, 50);
                    this.tb_tiempo.Location = new System.Drawing.Point(20+sizeAnterior, 80);
                    sizeAnterior += ((p.tiempoCPU * 1124) / tiempoTotal);

                    this.tb_tiempo.Text = tiempoActual.ToString();
                    retornoFCFS.Add(tiempoActual);
                    esperaFCFS.Add(tiempoActual - p.tiempoCPU);
                    //MessageBox.Show(tiempoActual.ToString() + " " + p.tiempoCPU);

                    this.tabPage2.Controls.Add(pb);
                    this.tabPage2.Controls.Add(tb_tiempo);
                } 
                contador++;
                if (c == 0)
                {
                    c = 1;
                }else if (c == 1) c = 0;
            }
        }
        public void ganttSJF()
        {
            int tiempoTotal = 0;
            int contador = 0;
            int sizeAnterior = 0;
            int tiempoActual = 0;

            int c = 0;
            int j = 0;

            Font myFont = new Font("Arial", 16);

            labelTitulo = new Label();
            labelTitulo.Text = "SJF";
            labelTitulo.Font = myFont;
            labelTitulo.Location = new System.Drawing.Point(20, 160);
            labelTitulo.AutoSize = true;

            this.tabPage2.Controls.Add(labelTitulo);
            listaSJF = listaProcesos.OrderBy(x => x.tiempoCPU).ToList();

            foreach (Trabajo p in listaSJF)
            {
                tiempoTotal += p.tiempoCPU;
            }
            foreach (Trabajo p in listaSJF)
            {
                tiempoActual += p.tiempoCPU;

                // Crea 2 PictureBox (bloque de Gantt), para el proceso y para el tiempo 
                miColor = listaColores.ElementAt(contador);

                pb = new PictureBox();
                this.pb.Size = new System.Drawing.Size(((p.tiempoCPU * 1124) / tiempoTotal), 30);
                this.pb.BackColor = miColor;
                this.pb.Paint += new PaintEventHandler((sender, e) =>
                {
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    e.Graphics.DrawString(p.numeroProceso.ToString(), myFont, Brushes.Black, (((p.tiempoCPU * 1124) / tiempoTotal) / 2), 3);
                });

                tb_tiempo = new TextBox();
                this.tb_tiempo.Size = new System.Drawing.Size(((p.tiempoCPU * 1124) / tiempoTotal), 30);
                this.tb_tiempo.BackColor = Color.LightGray;
                this.tb_tiempo.TextAlign = HorizontalAlignment.Center;
                this.tb_tiempo.Font = myFont;
                this.tb_tiempo.ForeColor = Color.Gray;

              

                if (contador == 0) // Si es el primer proceso su posición es fija (bloque de diagrama de Gantt)
                {
                    this.pb.Location = new System.Drawing.Point(20, 190);
                    sizeAnterior = ((p.tiempoCPU * 1124) / tiempoTotal);

                    this.tb_tiempo.Location = new System.Drawing.Point(20, 220);
                    this.tb_tiempo.Text = tiempoActual.ToString();
                    retornoSJF.Add(tiempoActual);
                    esperaSJF.Add(tiempoActual - p.tiempoCPU);

                    this.tabPage2.Controls.Add(pb);
                    this.tabPage2.Controls.Add(tb_tiempo);
                }
                else
                {
                    this.pb.Location = new System.Drawing.Point(20 + sizeAnterior, 190);
                    this.tb_tiempo.Location = new System.Drawing.Point(20 + sizeAnterior, 220);
                    sizeAnterior += ((p.tiempoCPU * 1124) / tiempoTotal);

                    this.tb_tiempo.Text = tiempoActual.ToString();
                    retornoSJF.Add(tiempoActual);
                    esperaSJF.Add(tiempoActual - p.tiempoCPU);

                    this.tabPage2.Controls.Add(pb);
                    this.tabPage2.Controls.Add(tb_tiempo);
                }

                contador++;

                if (c == 0)
                {
                    c = 1;
                }
                else if (c == 1) c = 0;
            }
        }
        // Dibuja el diagrama de Gantt utilizando el método Round Robin
        public void ganttRR() 
        {
            int tiempoTotal = 0;
            int contador = 0;
            int sizeAnterior = 0;

            int c = 0;
            int j = 0;

            Font myFont = new Font("Arial", 16);

            labelTitulo = new Label();
            labelTitulo.Text = "Round Robin";
            labelTitulo.Font = myFont;
            labelTitulo.Location = new System.Drawing.Point(20, 300);
            labelTitulo.AutoSize = true;

            this.tabPage2.Controls.Add(labelTitulo);

            foreach (Trabajo p in listaProcesos){
                copiaListaProcesos.Add(new Trabajo { numeroProceso = p.numeroProceso, prioridad = p.prioridad, tiempoCPU = p.tiempoCPU });
            }

            foreach (Trabajo p in copiaListaProcesos)
            {
                tiempoTotal += p.tiempoCPU;
            }

            for (int i = 0; i < tiempoTotal; i++)
            {
                if (j >= copiaListaProcesos.Count)
                {
                    j=0;        
                }

                miColor = listaColores.ElementAt(contador);
                
                label = new Label();
                this.label.Size = new System.Drawing.Size((1124 / tiempoTotal), 30);
                this.label.BackColor = miColor;
                this.label.Text = copiaListaProcesos.ElementAt(j).numeroProceso.ToString();
                listaProcesos.ElementAt(copiaListaProcesos.ElementAt(j).numeroProceso-1).retornoRR = contador+1;
                listaProcesos.ElementAt(copiaListaProcesos.ElementAt(j).numeroProceso - 1).esperaRR = contador + 1 - listaProcesos.ElementAt(copiaListaProcesos.ElementAt(j).numeroProceso - 1).tiempoCPU;

                this.label.TextAlign = ContentAlignment.MiddleCenter;
                this.label.Font = myFont;

                tb_tiempo = new TextBox();
                this.tb_tiempo.Size = new System.Drawing.Size((1124 / tiempoTotal), 30);
                this.tb_tiempo.BackColor = Color.LightGray;
                this.tb_tiempo.TextAlign = HorizontalAlignment.Center;
                this.tb_tiempo.Font = myFont;
                this.tb_tiempo.ForeColor = Color.Gray;

                if (contador == 0) // Si es el primer proceso su posición es fija (bloque de diagrama de Gantt)
                {
                    this.label.Location = new System.Drawing.Point(20, 330);
                    sizeAnterior = (1124 / tiempoTotal);

                    this.tb_tiempo.Location = new System.Drawing.Point(20, 360);
                    this.tb_tiempo.Text = (contador+1).ToString();

                    this.tabPage2.Controls.Add(label);
                    this.tabPage2.Controls.Add(tb_tiempo);
                }
                else
                {
                    this.label.Location = new System.Drawing.Point(20 + sizeAnterior, 330);
                    this.tb_tiempo.Location = new System.Drawing.Point(20 + sizeAnterior, 360);
                    sizeAnterior += (1124 / tiempoTotal);

                    this.tb_tiempo.Text = (contador+1).ToString();

                    this.tabPage2.Controls.Add(label);
                    this.tabPage2.Controls.Add(tb_tiempo);
                }



                copiaListaProcesos.ElementAt(j).tiempoCPU--;
               
                if (copiaListaProcesos.ElementAt(j).tiempoCPU == 0)
                {
                    copiaListaProcesos.RemoveAt(j);
                    j--;
                }

                j++;
                contador++;
            }

        }

        public void ganttPrioridades()
        {
            int tiempoTotal = 0;
            int contador = 0;
            int tiempoActual = 0;
            int sizeAnterior = 0;

            
            listaPrioridades = listaProcesos.OrderBy(x => x.prioridad).ToList();

            Font myFont = new Font("Arial", 16);

            labelTitulo = new Label();
            labelTitulo.Text = "Prioridades";
            labelTitulo.Font = myFont;
            labelTitulo.Location = new System.Drawing.Point(20, 440);
            labelTitulo.AutoSize = true;

            this.tabPage2.Controls.Add(labelTitulo);

            foreach (Trabajo p in listaPrioridades)
            {
                tiempoTotal += p.tiempoCPU;
            }


            foreach (Trabajo p in listaPrioridades)
            {
                tiempoActual += p.tiempoCPU;

                // Crea 2 PictureBox (bloque de Gantt), para el proceso y para el tiempo
                miColor = listaColores.ElementAt(contador);

                label = new Label();
                this.label.Size = new System.Drawing.Size(((p.tiempoCPU * 1124) / tiempoTotal), 30);
                this.label.BackColor = miColor;
                this.label.Text = p.numeroProceso.ToString();

                tb_tiempo = new TextBox();
                this.tb_tiempo.Size = new System.Drawing.Size(((p.tiempoCPU * 1124) / tiempoTotal), 30);
                this.tb_tiempo.BackColor = Color.LightGray;
                this.tb_tiempo.TextAlign = HorizontalAlignment.Center;
                this.tb_tiempo.Font = myFont;
                this.tb_tiempo.ForeColor = Color.Gray;


                if (contador == 0) // Si es el primer proceso su posición es fija (bloque de diagrama de Gantt)
                {
                    this.label.Location = new System.Drawing.Point(20, 470);
                    sizeAnterior = ((p.tiempoCPU * 1124) / tiempoTotal);

                    this.tb_tiempo.Location = new System.Drawing.Point(20, 500);
                    this.tb_tiempo.Text = tiempoActual.ToString();
                    retornoPrioridades.Add(tiempoActual);
                    esperaPrioridades.Add(tiempoActual - p.tiempoCPU);

                    this.tabPage2.Controls.Add(label);
                    this.tabPage2.Controls.Add(tb_tiempo);
                }
                else
                {
                    this.label.Location = new System.Drawing.Point(20 + sizeAnterior, 470);
                    this.tb_tiempo.Location = new System.Drawing.Point(20 + sizeAnterior, 500);
                    sizeAnterior += ((p.tiempoCPU * 1124) / tiempoTotal);

                    this.tb_tiempo.Text = tiempoActual.ToString();
                    retornoPrioridades.Add(tiempoActual);
                    esperaPrioridades.Add(tiempoActual - p.tiempoCPU);

                    this.tabPage2.Controls.Add(label);
                    this.tabPage2.Controls.Add(tb_tiempo);
                }

                contador++;
            }
        }

        public void poblarTabla(List<int> listaOpcines)
        {
            DataTable dt = new DataTable();

                if (listaOpcines.Count == 4)
                {
                    dt.Columns.Add("Trabajo");
                    dt.Columns.Add("FCFS");
                    dt.Columns.Add("SJF");
                    dt.Columns.Add("RR");
                    dt.Columns.Add("Prioridad");
                    for (int i=0; i<contadorProcesos; i++)
                    {
                        
                        dt.Rows.Add(new object[] { i + 1, retornoFCFS.ElementAt(i), retornoSJF.ElementAt(i), listaProcesos.ElementAt(i).retornoRR, retornoPrioridades.ElementAt(i) });
                    } 
                }else if(listaOpcines.Count == 1)
                {
                    if(listaOpcines.ElementAt(0) == 0)
                    {
                        dt.Columns.Add("Trabajo");
                        dt.Columns.Add("FCFS");
                        for(int i=0; i<contadorProcesos; i++)
                        {
                            dt.Rows.Add(new object[] { i + 1, retornoFCFS.ElementAt(i) });
                        }
                       
                    }else if(listaOpcines.ElementAt(0) == 1)
                    {
                        dt.Columns.Add("Trabajo");
                        dt.Columns.Add("SJF");
                        for(int i=0; i<contadorProcesos; i++)
                        {
                            dt.Rows.Add(new object[] { i + 1, retornoSJF.ElementAt(i) });
                        }              
                    }
                    else if (listaOpcines.ElementAt(0) == 2)
                    {
                        dt.Columns.Add("Trabajo");
                        dt.Columns.Add("RR");
                        for(int i=0; i < contadorProcesos; i++)
                        {
                            dt.Rows.Add(new object[] { i + 1, listaProcesos.ElementAt(i).retornoRR });
                        }
                        
                    }
                    else if (listaOpcines.ElementAt(0) == 3)
                    {
                        dt.Columns.Add("Trabajo");
                        dt.Columns.Add("Prioridades");
                        for(int i=0; i<contadorProcesos; i++)
                        {
                            dt.Rows.Add(new object[] { i + 1, retornoPrioridades.ElementAt(i) });
                        }
                        
                    }
                }else if (listaOpcines.Count == 2)
                {
                    if((listaOpcines.ElementAt(0) == 0) && (listaOpcines.ElementAt(1) == 1))
                    {
                        dt.Columns.Add("Trabajo");
                        dt.Columns.Add("FCFS");
                        dt.Columns.Add("SJF");
                        for(int i=0; i < contadorProcesos; i++)
                        {
                            dt.Rows.Add(new object[] { i + 1, retornoFCFS.ElementAt(i), retornoSJF.ElementAt(i) });
                        }
                        
                    }else if ((listaOpcines.ElementAt(0) == 0) && (listaOpcines.ElementAt(1) == 2))
                    {
                        dt.Columns.Add("Trabajo");
                        dt.Columns.Add("FCFS");
                        dt.Columns.Add("RR");
                        for(int i=0; i < contadorProcesos; i++)
                        {
                            dt.Rows.Add(new object[] { i + 1, retornoFCFS.ElementAt(i), listaProcesos.ElementAt(i).retornoRR });
                        }      
                    }
                    else if ((listaOpcines.ElementAt(0) == 0) && (listaOpcines.ElementAt(1) == 3))
                    {
                        dt.Columns.Add("Trabajo");
                        dt.Columns.Add("FCFS");
                        dt.Columns.Add("Prioridades");
                        for(int i=0; i<contadorProcesos; i++)
                        {
                            dt.Rows.Add(new object[] { i + 1, retornoFCFS.ElementAt(i), retornoPrioridades.ElementAt(i) });
                        }                  
                    }
                    else if ((listaOpcines.ElementAt(0) == 1) && (listaOpcines.ElementAt(1) == 2))
                    {
                        dt.Columns.Add("Trabajo");
                        dt.Columns.Add("SJF");
                        dt.Columns.Add("RR");
                        for(int i=0; i<contadorProcesos; i++)
                        {
                            dt.Rows.Add(new object[] { i + 1, retornoSJF.ElementAt(i), listaProcesos.ElementAt(i).retornoRR });
                        }                 
                    }
                    else if ((listaOpcines.ElementAt(0) == 1) && (listaOpcines.ElementAt(1) == 3))
                    {
                        dt.Columns.Add("Trabajo");
                        dt.Columns.Add("FCFS");
                        dt.Columns.Add("Prioridades");
                        for(int i=0; i<contadorProcesos; i++)
                        {
                            dt.Rows.Add(new object[] { i + 1, retornoSJF.ElementAt(i), retornoPrioridades.ElementAt(i) });
                        }   
                    }
                    else if ((listaOpcines.ElementAt(0) == 2) && (listaOpcines.ElementAt(1) == 3))
                    {
                        dt.Columns.Add("Trabajo");
                        dt.Columns.Add("RR");
                        dt.Columns.Add("Prioridades");
                        for(int i=0; i<contadorProcesos; i++)
                        {
                            dt.Rows.Add(new object[] { i + 1, listaProcesos.ElementAt(i).retornoRR, retornoPrioridades.ElementAt(i) });
                        }
                    }
                }
                else if (listaOpcines.Count == 3)
                {
                    if ((listaOpcines.ElementAt(0) == 0) && (listaOpcines.ElementAt(1) == 1) && (listaOpcines.ElementAt(2) == 2))
                    {
                        dt.Columns.Add("Trabajo");
                        dt.Columns.Add("FCFS");
                        dt.Columns.Add("SJF");
                        dt.Columns.Add("RR");
                        for(int i=0; i<contadorProcesos; i++)
                        {
                            dt.Rows.Add(new object[] { i + 1, retornoFCFS.ElementAt(i), retornoSJF.ElementAt(i), listaProcesos.ElementAt(i).retornoRR });
                        }
                    }else if ((listaOpcines.ElementAt(0) == 1) && (listaOpcines.ElementAt(1) == 2) && (listaOpcines.ElementAt(1) == 3))
                    {
                        dt.Columns.Add("Trabajo");
                        dt.Columns.Add("SJF");
                        dt.Columns.Add("RR");
                        dt.Columns.Add("Prioridades");
                        for(int i=0; i<contadorProcesos; i++)
                        {
                            dt.Rows.Add(new object[] { i + 1, retornoSJF.ElementAt(i), listaProcesos.ElementAt(i).retornoRR, retornoPrioridades.ElementAt(i) });
                        }                     
                    }
                    else if ((listaOpcines.ElementAt(0) == 0) && (listaOpcines.ElementAt(1) == 2) && (listaOpcines.ElementAt(2) == 3))
                    {
                        dt.Columns.Add("Trabajo");
                        dt.Columns.Add("FCFS");
                        dt.Columns.Add("RR");
                        dt.Columns.Add("Prioridades");
                        for(int i=0; i < contadorProcesos; i++)
                        {
                            dt.Rows.Add(new object[] { i + 1, retornoFCFS.ElementAt(i), listaProcesos.ElementAt(i).retornoRR, retornoPrioridades.ElementAt(i) });
                        }             
                    }
                    else if ((listaOpcines.ElementAt(0) == 0) && (listaOpcines.ElementAt(1) == 1) && (listaOpcines.ElementAt(2) == 3))
                    {
                        dt.Columns.Add("Trabajo");
                        dt.Columns.Add("FCFS");
                        dt.Columns.Add("SJF");
                        dt.Columns.Add("Prioridades");
                        for(int i=0; i<contadorProcesos; i++)
                        {
                            dt.Rows.Add(new object[] { i + 1, retornoFCFS.ElementAt(i), retornoSJF.ElementAt(i), retornoPrioridades.ElementAt(i) });
                        }              
                    }
                }

            dataGridView1.DataSource = dt;
        }

        public void tablaEspera(List<int> listaOpcines)
        {
            DataTable dt = new DataTable();

            if (listaOpcines.Count == 4)
            {
                dt.Columns.Add("Trabajo");
                dt.Columns.Add("FCFS");
                dt.Columns.Add("SJF");
                dt.Columns.Add("RR");
                dt.Columns.Add("Prioridad");
                for (int i = 0; i < contadorProcesos; i++)
                {
                    dt.Rows.Add(new object[] { i + 1, esperaFCFS.ElementAt(i), esperaSJF.ElementAt(i), listaProcesos.ElementAt(i).esperaRR, esperaPrioridades.ElementAt(i) });
                }
            }
            else if (listaOpcines.Count == 1)
            {
                if (listaOpcines.ElementAt(0) == 0)
                {
                    dt.Columns.Add("Trabajo");
                    dt.Columns.Add("FCFS");
                    for (int i = 0; i < contadorProcesos; i++)
                    {
                        dt.Rows.Add(new object[] { i + 1, esperaFCFS.ElementAt(i) });
                    }
                }
                else if (listaOpcines.ElementAt(0) == 1)
                {
                    dt.Columns.Add("Trabajo");
                    dt.Columns.Add("SJF");
                    for (int i = 0; i < contadorProcesos; i++)
                    {
                        dt.Rows.Add(new object[] { i + 1, esperaSJF.ElementAt(i) });
                    }
                }
                else if (listaOpcines.ElementAt(0) == 2)
                {
                    dt.Columns.Add("Trabajo");
                    dt.Columns.Add("RR");
                    for (int i = 0; i < contadorProcesos; i++)
                    {
                        dt.Rows.Add(new object[] { i + 1, listaProcesos.ElementAt(i).esperaRR });
                    }
                }
                else if (listaOpcines.ElementAt(0) == 3)
                {
                    dt.Columns.Add("Trabajo");
                    dt.Columns.Add("Prioridades");
                    for (int i = 0; i < contadorProcesos; i++)
                    {
                        dt.Rows.Add(new object[] { i + 1,esperaPrioridades.ElementAt(i) });
                    }
                }
            }
            else if (listaOpcines.Count == 2)
            {
                if ((listaOpcines.ElementAt(0) == 0) && (listaOpcines.ElementAt(1) == 1))
                {
                    dt.Columns.Add("Trabajo");
                    dt.Columns.Add("FCFS");
                    dt.Columns.Add("SJF");
                    for (int i = 0; i < contadorProcesos; i++)
                    {
                        dt.Rows.Add(new object[] { i + 1, esperaFCFS.ElementAt(i), esperaSJF.ElementAt(i) });
                    }
                }
                else if ((listaOpcines.ElementAt(0) == 0) && (listaOpcines.ElementAt(1) == 2))
                {
                    dt.Columns.Add("Trabajo");
                    dt.Columns.Add("FCFS");
                    dt.Columns.Add("RR");
                    for (int i = 0; i < contadorProcesos; i++)
                    {
                        dt.Rows.Add(new object[] { i + 1, esperaFCFS.ElementAt(i), listaProcesos.ElementAt(i).esperaRR });
                    }
                }
                else if ((listaOpcines.ElementAt(0) == 0) && (listaOpcines.ElementAt(1) == 3))
                {
                    dt.Columns.Add("Trabajo");
                    dt.Columns.Add("FCFS");
                    dt.Columns.Add("Prioridades");
                    for (int i = 0; i < contadorProcesos; i++)
                    {
                        dt.Rows.Add(new object[] { i + 1, esperaFCFS.ElementAt(i), esperaPrioridades.ElementAt(i) });
                    }
                }
                else if ((listaOpcines.ElementAt(0) == 1) && (listaOpcines.ElementAt(1) == 2))
                {
                    dt.Columns.Add("Trabajo");
                    dt.Columns.Add("SJF");
                    dt.Columns.Add("RR");
                    for (int i = 0; i < contadorProcesos; i++)
                    {
                        dt.Rows.Add(new object[] { i + 1, esperaSJF.ElementAt(i), listaProcesos.ElementAt(i).esperaRR });
                    }
                }
                else if ((listaOpcines.ElementAt(0) == 1) && (listaOpcines.ElementAt(1) == 3))
                {
                    dt.Columns.Add("Trabajo");
                    dt.Columns.Add("FCFS");
                    dt.Columns.Add("Prioridades");
                    for (int i = 0; i < contadorProcesos; i++)
                    {
                        dt.Rows.Add(new object[] { i + 1,esperaSJF.ElementAt(i), esperaPrioridades.ElementAt(i) });
                    }
                }
                else if ((listaOpcines.ElementAt(0) == 2) && (listaOpcines.ElementAt(1) == 3))
                {
                    dt.Columns.Add("Trabajo");
                    dt.Columns.Add("RR");
                    dt.Columns.Add("Prioridades");
                    for (int i = 0; i < contadorProcesos; i++)
                    {
                        dt.Rows.Add(new object[] { i + 1, listaProcesos.ElementAt(i).esperaRR, esperaPrioridades.ElementAt(i) });
                    }
                }
            }
            else if (listaOpcines.Count == 3)
            {
                if ((listaOpcines.ElementAt(0) == 0) && (listaOpcines.ElementAt(1) == 1) && (listaOpcines.ElementAt(2) == 2))
                {
                    dt.Columns.Add("Trabajo");
                    dt.Columns.Add("FCFS");
                    dt.Columns.Add("SJF");
                    dt.Columns.Add("RR");
                    for (int i = 0; i < contadorProcesos; i++)
                    {
                        dt.Rows.Add(new object[] { i + 1, esperaFCFS.ElementAt(i), esperaSJF.ElementAt(i), listaProcesos.ElementAt(i).esperaRR });
                    }
                }
                else if ((listaOpcines.ElementAt(0) == 1) && (listaOpcines.ElementAt(1) == 2) && (listaOpcines.ElementAt(1) == 3))
                {
                    dt.Columns.Add("Trabajo");
                    dt.Columns.Add("SJF");
                    dt.Columns.Add("RR");
                    dt.Columns.Add("Prioridades");
                    for (int i = 0; i < contadorProcesos; i++)
                    {
                        dt.Rows.Add(new object[] { i + 1, esperaSJF.ElementAt(i), listaProcesos.ElementAt(i).esperaRR, esperaPrioridades.ElementAt(i) });
                    }
                }
                else if ((listaOpcines.ElementAt(0) == 0) && (listaOpcines.ElementAt(1) == 2) && (listaOpcines.ElementAt(2) == 3))
                {
                    dt.Columns.Add("Trabajo");
                    dt.Columns.Add("FCFS");
                    dt.Columns.Add("RR");
                    dt.Columns.Add("Prioridades");
                    for (int i = 0; i < contadorProcesos; i++)
                    {
                        dt.Rows.Add(new object[] { i + 1, esperaFCFS.ElementAt(i), listaProcesos.ElementAt(i).esperaRR, esperaPrioridades.ElementAt(i) });
                    }
                }
                else if ((listaOpcines.ElementAt(0) == 0) && (listaOpcines.ElementAt(1) == 1) && (listaOpcines.ElementAt(2) == 3))
                {
                    dt.Columns.Add("Trabajo");
                    dt.Columns.Add("FCFS");
                    dt.Columns.Add("SJF");
                    dt.Columns.Add("Prioridades");
                    for (int i = 0; i < contadorProcesos; i++)
                    {
                        dt.Rows.Add(new object[] { i + 1, esperaFCFS.ElementAt(i), esperaSJF.ElementAt(i), esperaPrioridades.ElementAt(i) });
                    }
                }
            }
            dataGridView2.DataSource = dt;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
