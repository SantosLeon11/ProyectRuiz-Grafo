using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProyectRuiz_Grafo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Grafo grafo;
        public MainWindow()
        {
            InitializeComponent();
            InitializeComponent();
            // Inicializa el grafo
            grafo = new Grafo();
            grafo.AddVertice(1, 25, 40);
            grafo.AddVertice(2, 100, 40);
            grafo.AddVertice(3, 150, 140);
            grafo.AddVertice(4, 200, 115);
            grafo.AddVertice(5, 225, 65);
            grafo.AddVertice(6, 300, 160);
            grafo.AddVertice(7, 350, 100);

            //Se agregan pesos a las lineas
            grafo.AddPeso(1, 2, 2);
            grafo.AddPeso(2, 3, 7);
            grafo.AddPeso(1, 3, 10);
            grafo.AddPeso(3, 4, 1);
            grafo.AddPeso(4, 5, 5);
            grafo.AddPeso(4, 6, 6);
            grafo.AddPeso(5, 7, 9);
            grafo.AddPeso(6, 7, 11);

            DibujarGrafo(grafo);

            // Se llama a la funcion de dijkstra
            int VerticeInicio = 1;
            int VerticeFinal = 7;
            var CaminoCorto = DijkstraCaminoCorto(grafo, VerticeInicio, VerticeFinal);

            // Se muestra el camino mas corto
            MostrarCaminoCorto(CaminoCorto);
        }
        public class Vertice
        {
            public int Id { get; }
            public double X { get; }
            public double Y { get; }

            public Vertice(int id, double x, double y)
            {
                Id = id;
                X = x;
                Y = y;
            }
        }
        public class Grafo
        {
            public Dictionary<int, List<(Vertice vecino, int peso)>> ListaAdjacencia { get; }

            public Grafo()
            {
                ListaAdjacencia = new Dictionary<int, List<(Vertice, int)>>();
            }

            // Se agrgan vertices y pesos
            public void AddVertice(int vertexId, double x, double y)
            {
                if (!ListaAdjacencia.ContainsKey(vertexId))
                    ListaAdjacencia[vertexId] = new List<(Vertice, int)>();

                ListaAdjacencia[vertexId].Add((new Vertice(vertexId, x, y), 0));
            }

            public void AddPeso(int sourceId, int destinationId, int weight)
            {
                if (!ListaAdjacencia[sourceId].Any(v => v.Item1.Id == destinationId))
                {
                    ListaAdjacencia[sourceId].Add((ListaAdjacencia[destinationId][0].Item1, weight));
                }
            }
        }
        private void DibujarGrafo(Grafo grafo)
        {
            // Se limpia el dibujo anterior
            canvas.Children.Clear();

            foreach (var Dvertices in grafo.ListaAdjacencia.Values)
            {
                Vertice vertice = Dvertices[0].Item1;
                Ellipse ellipse = new Ellipse
                {
                    Width = 20,
                    Height = 20,
                    //Color de los puntos
                    Fill = Brushes.CadetBlue
                };

                Canvas.SetLeft(ellipse, vertice.X);
                Canvas.SetTop(ellipse, vertice.Y);
                canvas.Children.Add(ellipse);

                foreach (var (vecino, peso) in Dvertices.Skip(1))
                {
                    Line line = new Line
                    {
                        X1 = vertice.X + 10, 
                        Y1 = vertice.Y + 10,
                        X2 = vecino.X + 10, 
                        Y2 = vecino.Y + 10,
                        //Color de lineas
                        Stroke = Brushes.Gray,
                        StrokeThickness = 2
                    };

                    canvas.Children.Add(line);

                    TextBlock weightLabel = new TextBlock
                    {
                        Text = peso.ToString(),
                        //Color del peso
                        Foreground = Brushes.AliceBlue
                    };

                    Canvas.SetLeft(weightLabel, (vertice.X + vecino.X) / 2);
                    Canvas.SetTop(weightLabel, (vertice.Y + vecino.Y) / 2);
                    canvas.Children.Add(weightLabel);
                }
            }
        }

        public List<int> DijkstraCaminoCorto(Grafo grafo, int VerticeInicio, int VerticeFinal)
        {
            Dictionary<int, int> distance = new Dictionary<int, int>();
            Dictionary<int, int> previous = new Dictionary<int, int>();
            List<int> unvisitedVertices = new List<int>();

            foreach (var vertexData in grafo.ListaAdjacencia.Values)
            {
                Vertice vertice = vertexData[0].Item1;
                distance[vertice.Id] = int.MaxValue;
                previous[vertice.Id] = -1;
                unvisitedVertices.Add(vertice.Id);
            }

            distance[VerticeInicio] = 0;

            int sum = 0;
            while (unvisitedVertices.Count > 0)
            {
                int currentVertexId = unvisitedVertices.OrderBy(v => distance[v]).First();
                unvisitedVertices.Remove(currentVertexId);

                if (distance[currentVertexId] == int.MaxValue)
                {
                    break;
                }

                foreach (var (vecino, peso) in grafo.ListaAdjacencia[currentVertexId].Skip(1))
                {
                    int altDistance = distance[currentVertexId] + peso;
                    if (altDistance < distance[vecino.Id])
                    {
                        distance[vecino.Id] = altDistance;
                        previous[vecino.Id] = currentVertexId;
                        sum += peso;
                    }
                }
            }
            List<int> caminocorto = new List<int>();
            int vertexAt = VerticeFinal;
            while (vertexAt != -1)
            {
                caminocorto.Insert(0, vertexAt);
                vertexAt = previous[vertexAt];
            }
            totalLabel.Text = sum.ToString();
            return caminocorto;
        }
        private void MostrarCaminoCorto(List<int> caminocorto)
        {
            if (caminocorto.Count == 0)
            {
                CaminoCorto.Text = "Camino no encontrado.";
                return;
            }

            StringBuilder pathBuilder = new StringBuilder();
            foreach (int vertexId in caminocorto)
            {
                pathBuilder.Append(vertexId).Append(" -> ");
            }
            CaminoCorto.Text = "Camino corto: " + pathBuilder.ToString();
        }
    }
}
