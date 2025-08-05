using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;
using System.Data.SqlClient;// conexion sql


namespace Negocio
{
    public class ConexionCatalogo
    {
        public List<Catalogo> listar()
     {
         List<Catalogo> lista = new List<Catalogo>(); 
         SqlConnection conexion = new SqlConnection();//OBJETOS configurados par aluego hacer la lectura solo en este metodo!
         SqlCommand comando = new SqlCommand();
         SqlDataReader lector;

        try 
	  {	        
		 conexion.ConnectionString = "server=.\\SQLEXPRESS; database=CATALOGO_DB; integrated security=True";  
         comando.CommandType = System.Data.CommandType.Text;
         comando.CommandText= "Select A.Id,Codigo, Nombre, A.Descripcion, ImagenUrl, Precio, C.Descripcion Inventario,M.Descripcion Firmas, A.IdMarca,A.IdCategoria from ARTICULOS A, CATEGORIAS C, MARCAS M where C.Id= A.IdCategoria and M.Id = A.IdMarca";
         comando.Connection = conexion;

                conexion.Open();
                lector = comando.ExecuteReader();

                while(lector.Read())
                {
                    Catalogo aux = new Catalogo();
                    aux.Id = (int)lector["Id"];
                    aux.Codigo = (string)lector["Codigo"];
                    aux.Nombre = (string)lector["Nombre"];
                    aux.Descripcion = (string)lector["Descripcion"];
                    aux.ImagenUrl = (string)lector["ImagenUrl"];
                    aux.Precio = Convert.ToDecimal(lector["Precio"]);

                    aux.Inventario = new Categorias();
                    aux.Inventario.Id = (int)lector["IdMarca"];
                    aux.Inventario.Descripcion = (string)lector["Inventario"];
                    aux.Firmas = new Marcas();
                    aux.Firmas.Descripcion = (string)lector["Firmas"];
                    aux.Firmas.Id = (int)lector["Idcategoria"];
                    

                    lista.Add(aux);
                }
                conexion.Close();
                return lista;
          }
	      catch (Exception ex)
	      {

		  throw;
	      }

         }

        public void agregarCatalogo(Catalogo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {

                datos.setearConsulta("insert into ARTICULOS (Codigo, Nombre,Descripcion,ImagenUrl,Precio, IdMarca,IdCategoria ) values(@Codigo, @Nombre, @Descripcion, @ImagenUrl, @Precio, @IdMarca, @IdCategoria)");
                datos.setearParametros("@Codigo", nuevo.Codigo);
                datos.setearParametros("@Nombre", nuevo.Nombre);
                datos.setearParametros("@Descripcion", nuevo.Descripcion);
                datos.setearParametros("@ImagenUrl", nuevo.ImagenUrl);
                datos.setearParametros("@Precio", nuevo.Precio);
                datos.setearParametros("@IdMarca", nuevo.Firmas.Id);
                datos.setearParametros("@IdCategoria", nuevo.Inventario.Id);
                
               


                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }

        }
        public void modificarCatalogo(Catalogo modificar)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("update ARTICULOS set Codigo= @Codigo, Nombre = @Nombre, Descripcion = @Descripcion,ImagenUrl = @ImagenUrl, Precio = @Precio, IdMarca = @IdMarca, IdCategoria = @IdCategoria where Id= @Id ");
                datos.setearParametros("@Codigo", modificar.Codigo);
                datos.setearParametros("@Nombre", modificar.Nombre);
                datos.setearParametros("@Descripcion", modificar.Descripcion);
                datos.setearParametros("@ImagenUrl", modificar.ImagenUrl);
                datos.setearParametros("@Precio", modificar.Precio);
                datos.setearParametros("@IdMarca", modificar.Firmas.Id);
                datos.setearParametros("@IdCategoria", modificar.Inventario.Id);
                datos.setearParametros("@Id", modificar.Id);




                datos.ejecutarAccion();

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public List<Catalogo> filtrar(string campo, string criterio, string filtro)
        {
            List<Catalogo> lista = new List<Catalogo>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                string consulta = "Select A.Id,Codigo, Nombre, A.Descripcion, ImagenUrl, Precio, C.Descripcion Inventario,M.Descripcion Firmas, A.IdMarca,A.IdCategoria from ARTICULOS A, CATEGORIAS C, MARCAS M where C.Id= A.IdCategoria and M.Id = A.IdMarca and ";
                if (campo == "Precio")
                {

                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "Precio > " + filtro;
                            break;
                        case "Menor a":
                            consulta += "Precio < " + filtro;
                            break;
                        default:
                            consulta += "Precio = " + filtro;
                            break;
                    }


                }
                else if (campo == "Nombre")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Nombre like '" + filtro + "%' ";
                            break;
                        case "Termina con":
                            consulta += "Nombre like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "Nombre like '%" + filtro + "%'";
                            break;
                    }
                }
                else
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "M.Descripcion like '" + filtro + "%' ";
                            break;
                        case "Termina con":
                            consulta += "M.Descripcion like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "M.Descripcion like '%" + filtro + "%'";
                            break;
                    }

                }

                datos.setearConsulta(consulta);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Catalogo aux = new Catalogo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    aux.ImagenUrl = (string)datos.Lector["ImagenUrl"];
                    aux.Precio = Convert.ToDecimal(datos.Lector["Precio"]);

                    aux.Inventario = new Categorias();
                    aux.Inventario.Id = (int)datos.Lector["IdMarca"];
                    aux.Inventario.Descripcion = (string)datos.Lector["Inventario"];
                    aux.Firmas = new Marcas();
                    aux.Firmas.Descripcion = (string)datos.Lector["Firmas"];
                    aux.Firmas.Id = (int)datos.Lector["Idcategoria"];


                    lista.Add(aux);
                }
                

                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void eliminar (int id)
        {
            AccesoDatos datos = new AccesoDatos();
                try
            {
                datos.setearConsulta("delete from ARTICULOS where Id=@Id");
                datos.setearParametros("@Id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
