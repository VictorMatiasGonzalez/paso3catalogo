using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dominio;
using Negocio;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO;
using System.Configuration;

namespace Presentacion
{
    public partial class FrmAltaCatalogo : Form
    {//Event2
        private Catalogo nuevocatalogo = null;
        private OpenFileDialog archivo = null;
        public FrmAltaCatalogo()
        {
            InitializeComponent();
        }

        public FrmAltaCatalogo(Catalogo catalogo)//Modificar
        {
            InitializeComponent();
            this.nuevocatalogo = catalogo;
            Text = "Modificar Articulo";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void btAceptar_Click(object sender, EventArgs e)
        {
           
            ConexionCatalogo conexion = new ConexionCatalogo();// ver eso para detalle
            try
            {
                if (nuevocatalogo == null)
                    nuevocatalogo = new Catalogo();

                nuevocatalogo.Codigo = txtCodigoArticulo.Text;
                nuevocatalogo.Nombre = txtNombre.Text;
                nuevocatalogo.Descripcion = txtDescripcion.Text;
                nuevocatalogo.ImagenUrl = txtImagen.Text;
                string textPrecio = txtPrecio.Text.Trim();

                
                if (textPrecio.Contains(","))
                {
                    MessageBox.Show(" No se aceptan comas.");
                    return;
                }

                
                if (!Regex.IsMatch(textPrecio, @"^\d+(\.\d{1,2})?$"))
                {
                    MessageBox.Show("Solo se permiten números . No ingreses letras ni símbolos.");
                    return;
                }

                
                if (!decimal.TryParse(textPrecio, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precio) || precio <= 0)
                {
                    MessageBox.Show("Verificá que el precio ingresado sea válido y mayor a cero.");
                    return;
                }

               
                nuevocatalogo.Precio = precio;

                
                txtPrecio.Text = precio.ToString(CultureInfo.InvariantCulture);


                nuevocatalogo.Firmas = (Marcas)cbMarca.SelectedItem;
                nuevocatalogo.Inventario = (Categorias)cbCategoria.SelectedItem;

                if (nuevocatalogo.Id != 0 )
               {
                    conexion.modificarCatalogo(nuevocatalogo);
                    MessageBox.Show("Articulo modificado correctamente");
                }

                else
                {
                    conexion.agregarCatalogo(nuevocatalogo);
                    MessageBox.Show("Articulo agregado correctamente");
                }
                
                // guardar imagen
               if (archivo != null)
                {
                    string destino = Path.Combine(ConfigurationManager.AppSettings["poketfoto"], archivo.SafeFileName);
                    if (!File.Exists(destino))
                        File.Copy(archivo.FileName, destino);
                    txtImagen.Text = destino;
                    cargarImagen(destino);
                }
                Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void FrmAltaCatalogo_Load(object sender, EventArgs e)
        {
            ConexionMarcas conexionMarcas = new ConexionMarcas();
            ConexionCatagorias conexionCategorias = new ConexionCatagorias();
            try
            {
                cbMarca.DataSource = conexionMarcas.listar();
                cbMarca.ValueMember = "Id";
                cbMarca.DisplayMember = "Descripcion";
                cbCategoria.DataSource = conexionCategorias.listar();
                cbCategoria.ValueMember = "Id";
                cbCategoria.DisplayMember = "Descripcion";

                if(nuevocatalogo != null)
                {
                    txtCodigoArticulo.Text = nuevocatalogo.Codigo.ToString();
                    txtNombre.Text = nuevocatalogo.Nombre;
                    txtDescripcion.Text = nuevocatalogo.Descripcion;
                    txtImagen.Text = nuevocatalogo.ImagenUrl;
                    txtPrecio.Text = nuevocatalogo.Precio.ToString();
                    cargarImagen(nuevocatalogo.ImagenUrl);
                    cbMarca.SelectedValue = nuevocatalogo.Firmas.Id;
                    cbCategoria.SelectedValue = nuevocatalogo.Inventario.Id;
                    
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void txtImagen_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtImagen.Text);
        }
        private void cargarImagen(string imagenn)
        {

            try
            {
                pBCatalogo.Load(imagenn);
            }
            catch (Exception ex)
            {

                pBCatalogo.Load("https://developers.elementor.com/docs/assets/img/elementor-placeholder-image.png");
            }
        }

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
             archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg;|png|*.png";
            if (archivo.ShowDialog() == DialogResult.OK)
            {
                txtImagen.Text = archivo.FileName;
                cargarImagen(archivo.FileName);
                  
            }
        }
    }
}
