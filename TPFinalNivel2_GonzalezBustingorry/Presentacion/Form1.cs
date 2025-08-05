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

namespace Presentacion
{//EVENT
    public partial class Form1 : Form
    {
        private List<Catalogo> listaArticulos;//atributo privado para manipular lista libremente(conexion.listar)
        public Form1()
        {
            InitializeComponent();
        }
        
        

        private void Form1_Load(object sender, EventArgs e)//LOAD
        {
            cargar();
            cbCampo.Items.Add("Nombre");
            cbCampo.Items.Add("Marca");
            cbCampo.Items.Add("Precio");
        }

        private void dgvCatalogo_SelectionChanged(object sender, EventArgs e)// propiedad de dgv para  cambiar seleccion
        {
            if (dgvCatalogo.CurrentRow != null && dgvCatalogo.CurrentRow.DataBoundItem != null)
            {
                Catalogo seleccionado = (Catalogo)dgvCatalogo.CurrentRow.DataBoundItem;// objeto transformado en articulo del catalogo
                cargarImagen(seleccionado.ImagenUrl);
            }
        }
        private void cargar()
        {
            ConexionCatalogo conexion = new ConexionCatalogo();

            try
            {
                listaArticulos = conexion.listar();// con listaArticulos puedo trabajar mas facil
                dgvCatalogo.DataSource = listaArticulos;//datasource> mostrmae este dato/dgv lo ordena automaticamente en columnas
                dgvCatalogo.Columns["Id"].Visible = false;
                dgvCatalogo.Columns["ImagenUrl"].Visible = false;
                cargarImagen(listaArticulos[0].ImagenUrl);//carga imagen en el picture box
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
        private void cargarImagen(string imagen)
        {
            
            try
            {
                pbCatalogo.Load(imagen);
            }
            catch (Exception ex)
            {

                pbCatalogo.Load("https://developers.elementor.com/docs/assets/img/elementor-placeholder-image.png");
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)//AGREGAR
        {
            FrmAltaCatalogo alta =  new FrmAltaCatalogo();
            alta.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)//MODIFICAR
        {
            Catalogo seleccionado;
            seleccionado = (Catalogo)dgvCatalogo.CurrentRow.DataBoundItem;

            FrmAltaCatalogo modificar = new FrmAltaCatalogo(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            ConexionCatalogo conexion = new ConexionCatalogo();
            Catalogo seleccionado;
            
            try
            { 

                DialogResult respuesta = MessageBox.Show("¿Encerio queres eliminarlo?","Eliminando",MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
                if (respuesta == DialogResult.Yes)

                {
                    seleccionado = (Catalogo)dgvCatalogo.CurrentRow.DataBoundItem;
                    conexion.eliminar(seleccionado.Id);
                    cargar();
                }
                


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnFiltro_Click(object sender, EventArgs e)

        {
            ConexionCatalogo conexion = new ConexionCatalogo();
            try
            {
                if (cbCampo.SelectedItem == null || cbCriterio.SelectedItem == null)
                {
                    MessageBox.Show("Seleccione un item a filtrar");
                    return;
                }
                string filtro = cbFiltroAvanzado.Text;

                string campo = cbCampo.SelectedItem.ToString();
                string criterio = cbCriterio.SelectedItem.ToString();
                
                if (filtro == "")

                {
                    MessageBox.Show("Por favor complete el campo filtro");
                    return;
                }
                if (campo == "Precio")
                    if(!soloNumeros(cbFiltroAvanzado.Text))
                    {
                        MessageBox.Show("Solo escriba numeros por favor");
                            return ;
                    }
                if (campo == "Marca" || campo == "Nombre")
                {
                    if (!soloLetras(cbFiltroAvanzado.Text))
                    {
                        MessageBox.Show("Solo escriba letras por favor");
                        return;
                    }
                }

                dgvCatalogo.DataSource = conexion.filtrar(campo, criterio, filtro);
              
            }

            
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
        private bool soloNumeros( string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                    return false;
            }
           return true;
        }
        private bool soloLetras (string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsLetter(caracter)))
                    return false;
            }
            return true;
        }

            

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            List<Catalogo> listafiltrada;
           
            
            listafiltrada = listaArticulos;
            dgvCatalogo.DataSource = null;
            dgvCatalogo.DataSource = listafiltrada;
            dgvCatalogo.Columns["Id"].Visible = false;
            dgvCatalogo.Columns["ImagenUrl"].Visible = false;
        }

        

        private void txtFiltroRapido_TextChanged(object sender, EventArgs e)
        {
            List<Catalogo> listafiltrada;

            if (txtFiltroRapido.Text.Length >= 2)
            {
                listafiltrada = listaArticulos.FindAll(x => x.Nombre.ToUpper().Contains(txtFiltroRapido.Text.ToUpper()) || x.Firmas.Descripcion.ToUpper().Contains(txtFiltroRapido.Text.ToUpper()));
            }
            else
                listafiltrada = listaArticulos;

            dgvCatalogo.DataSource = null;
            dgvCatalogo.DataSource = listafiltrada;
            dgvCatalogo.Columns["Id"].Visible = false;
            dgvCatalogo.Columns["ImagenUrl"].Visible = false;
        }

        private void cbCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cbCampo.SelectedItem.ToString();
            if (opcion == "Precio")
            {
                cbCriterio.Items.Clear();
                cbCriterio.Items.Add("Mayor a");
                cbCriterio.Items.Add("Menor a");
                cbCriterio.Items.Add("Igual a");
            }
            else
            {
                cbCriterio.Items.Clear();
                cbCriterio.Items.Add("Empieza con");
                cbCriterio.Items.Add("Termina con");
                cbCriterio.Items.Add("Abarca");
            }
        }

        
    }
    
}
