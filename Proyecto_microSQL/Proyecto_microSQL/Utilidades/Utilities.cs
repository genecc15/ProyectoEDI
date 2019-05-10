﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using EstructurasDeDatos;

namespace Proyecto_microSQL.Utilidades
{
    class Utilities
    {
        string path;
        List<string> palabrasReservadas;
        List<string> palabrasReemplazo;
        List<string> tiposReservados;
        List<string> tiposReemplazo;
        CrearTabla tabla = new CrearTabla();
        InsertInto insertar = new InsertInto();
        Selection seleccion = new Selection();
        string nombreTabla = string.Empty;
        string[] deleteTabla = new string[2];

        public void setPath(string p)
        {
            path = p;
        }

        public CrearTabla Tabla
        {
            get
            {
                return tabla;
            }

            set
            {
                tabla = value;
            }
        }

        public InsertInto Insertar1
        {
            get
            {
                return insertar;
            }

            set
            {
                insertar = value;
            }
        }

        public Selection Seleccion
        {
            get
            {
                return seleccion;
            }

            set
            {
                seleccion = value;
            }
        }

        public string NombreTablaEliminar
        {
            get
            {
                return nombreTabla;
            }

            set
            {
                nombreTabla = value;
            }
        }

        public string[] DeleteTabla
        {
            get
            {
                return deleteTabla;
            }

            set
            {
                deleteTabla = value;
            }
        }

        #region CreateStuff
        public void crearFolder() //crear todas las carpetas
        {
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(path + "\\arbolesb");
            Directory.CreateDirectory(path + "\\microSQL");
            Directory.CreateDirectory(path + "\\tablas");
        }

        public bool CrearDefault()
        {
            try
            {
                FileStream fs = File.Create(path + "microSQL\\microSQL.ini");
                fs.Close();
                using (StreamWriter file = new StreamWriter(path + "microSQL\\microSQL.ini", true))
                {
                    file.WriteLine("SELECT,SELECT");
                    file.WriteLine("FROM,FROM");
                    file.WriteLine("DELETE,DELETE");
                    file.WriteLine("WHERE,WHERE");
                    file.WriteLine("CREATE TABLE,CREATE TABLE");
                    file.WriteLine("DROP TABLE,DROP TABLE");
                    file.WriteLine("INSERT INTO,INSERT INTO");
                    file.WriteLine("VALUES,VALUES");
                    file.WriteLine("GO,GO");
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AlmacenarComandos(List<string> comandos)
        {
            try
            {
                if (File.Exists(path + "microSQL\\microSQL.ini"))
                {
                    File.Delete(path + "microSQL\\microSQL.ini");
                }
                                
                FileStream fs = File.Create(path + "microSQL\\microSQL.ini");
                fs.Close();
                using (StreamWriter file = new StreamWriter(path + "microSQL\\microSQL.ini", true))
                {
                    for(int i = 0; i < comandos.Count; i++)
                    {
                        file.WriteLine(comandos[i]);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<string> CargarComando()
        {
            try
            {
                string[] line;
                List<string> comandolst = new List<string>();

                palabrasReservadas = new List<string>();
                palabrasReemplazo = new List<string>();

                string[] lines = File.ReadAllLines(path + "microSQL\\microSQL.ini");
                for (int i = 0; i < lines.Length; i++)
                {
                    line = lines[i].Split(',');
                    palabrasReservadas.Add(line[1]);
                    palabrasReemplazo.Add(string.Join("", line[1].Split(' ')));
                }

                return palabrasReservadas;
            }
            catch
            {
                return null;
            }
        }

        public List<string> CargarTiposDefault()
        {
            tiposReservados = new List<string>();
            tiposReemplazo = new List<string>();

            tiposReservados.Add("INT PRIMARY KEY");
            tiposReservados.Add("VARCHAR(100)");
            tiposReservados.Add("DATETIME");
            tiposReservados.Add("INT");

            tiposReemplazo.Add("INTPRIMARYKEY");
            tiposReemplazo.Add("VARCHAR(100)");
            tiposReemplazo.Add("DATETIME");
            tiposReemplazo.Add("INT");

            return tiposReservados;
        }

        public List<string> ObtenerReemplazo()
        {
            return palabrasReemplazo;
        }

        public bool CrearArchivoTabla(string id, List<string> columns, string tablename)
        {
            try
            {
                FileStream fs = File.Create(path + "tablas\\" + tablename + ".tabla");
                fs.Close();
                for (int i = 0; i < columns.Count(); i++)
                {
                    var splitted = columns[i].Split(new[] { ' ' }, 2);
                    columns[i] = splitted[0] + " " + splitted[1];
                }
                using (StreamWriter file = new StreamWriter(path + "tablas\\" + tablename + ".tabla", true))
                {
                    file.WriteLine(id + "," + string.Join(",", columns));
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CrearArbol(string treeName, string id, List<string> types)
        {
            try
            {
                //****Moficar Dato[0].ToString en fabrica****
                BTree<int, Fila> tree = new BTree<int, Fila>(treeName, 5, path + "\\arbolesb");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void crearTabla(CrearTabla tabla)
        {
            CrearArchivoTabla(tabla.Id, tabla.Names, tabla.TableName.Trim());
            CrearArbol(tabla.TableName.Trim(), tabla.Id, tabla.Types);
        }

        #endregion

        #region Errores De Sintaxis

        /// <summary>
        /// Verifica la sintaxis cuando se ejecuta el comando Create Table.
        /// </summary>
        /// <param name="datos"></param>
        /// <returns>Retorna el numero de error. (0 significa que no hay errores) </returns>
        public int VerificarSintaxisCrearTabla(List<string> datos)
        {

            if (!datos.Contains(string.Join("", tiposReservados[0].Split(' '))))
            {
                //No hay existencia de un INT PRIMARY KEY
                return 8;
            }

            //Verificar la existencia de la llave de apertura.
            if (datos[1] != "(" && !datos.Contains(")"))
            {
                //Error de llave de apertura no encontrado
                return 3;
            }

            //En caso que exista algo que no es el operador {
            if (datos[1] != "(")
            {
                //Error de espacios de los nombres de las variables
                return 9;
            }

            //INT PRIMARY KEY distinto de ID
            if(datos[2] != "ID" || datos[3] != tiposReemplazo[0])
            {
                return 28;
            }

            //En caso el ultimo elemento no es la llave de cierre
            if (datos[datos.Count - 1] != ")" && !datos.Contains(")"))
            {
                return 4;
            }

            //En caso que exista la llave de cierre pero hay mas elementos despues de esto.
            if (datos[datos.Count - 1] != ")")
            {
                return 1;
            }

            //No se crea la tabla debido a que ya existe en el contexto.
            if(ExisteTabla(datos[0]))
            {
                return 27;
            }
                
            CrearTabla nuevaTabla = new CrearTabla();
            int[] counts = new int[3];
            nuevaTabla.TableName = datos[0];

            bool flag = false;

            for (int i = 2; i < datos.Count - 2; i++)
            {
                flag = false;

                //Omitiendo el nombre y la llave de apertura
                for (int j = 0; j < tiposReemplazo.Count; j++)
                {
                    if (datos[i] == tiposReemplazo[j])
                    {
                        return 10;
                    }

                    if (datos[i + 1] == tiposReemplazo[j])
                    {
                        flag = true;
                    }
                }

                if (flag)
                {
                    if (datos[i + 1] == tiposReemplazo[0])
                    {
                        if (nuevaTabla.Id == string.Empty)
                        {
                            nuevaTabla.Id = datos[i];
                        }
                        else
                        {
                            //Sobrepaso la cantidad de elementos admitidos del tipo de dato
                            nuevaTabla = null;
                            return 12;
                        }
                    }

                    if (datos[i + 1] == tiposReemplazo[1])
                    {
                        if (counts[0] < 3)
                        {
                            counts[0]++;
                            nuevaTabla.Names.Add(datos[i] + " " + tiposReservados[1]);
                            nuevaTabla.Types.Add(tiposReservados[1]);
                        }
                        else
                        {
                            //Sobrepaso la cantidad de elementos admitidos del tipo de dato
                            nuevaTabla = null;
                            return 13;
                        }

                    }

                    if (datos[i + 1] == tiposReemplazo[2])
                    {
                        if (counts[1] < 3)
                        {
                            counts[1]++;
                            nuevaTabla.Names.Add(datos[i] + " " + tiposReservados[2]);
                            nuevaTabla.Types.Add(tiposReservados[2]);
                        }
                        else
                        {
                            //Sobrepaso la cantidad de elementos admitidos del tipo de dato
                            nuevaTabla = null;
                            return 13;
                        }
                    }

                    if (datos[i + 1] == tiposReemplazo[3] && counts[2] < 3)
                    {
                        if (counts[2] < 3)
                        {
                            counts[2]++;
                            nuevaTabla.Names.Add(datos[i] + " " + tiposReservados[3]);
                            nuevaTabla.Types.Add(tiposReservados[3]);
                        }
                        else
                        {
                            //Sobrepaso la cantidad de elementos admitidos del tipo de dato
                            nuevaTabla = null;
                            return 13;
                        }
                    }
                }
                else
                {
                    //Error de tipo de dato
                    return 11;
                }

                i++;
            }

            tabla = nuevaTabla;
            //TablasPorCrear.Enqueue(nuevaTabla);

            return 0;
        }

        /// <summary>
        /// Verifica la sintaxis cuando se ejecuta el comando Select.
        /// </summary>
        /// <param name="datos"></param>
        /// <returns>Retorna el numero de error. (0 significa que no hay errores) </returns>
        public int VerificiarSintaxisSelect(List<string> datos)
        {
            int i = 0;
            int aux = 0;
            int aux2 = 0;
            int aux3 = 0;
            int aux4 = 0;
            Selection nuevaSeleccion = new Selection();
            List<string> columnas = new List<string>();
            List<string> filtro = new List<string>();
            string nombreTabla = string.Empty;
          
            if (!datos.Contains(palabrasReemplazo[1]))
            {
                //No contiene el comando from
                return 30;
            }

            //Busca algun elemento 'WHERE O FROM' repetido
            //tambien verifica que no se esten usando comandos que no corresponden a la funcion
            for (i = 0; i < datos.Count; i++)
            {
                if (datos[i] == palabrasReemplazo[7])
                {
                    //Error de comandos que no corresponden a la funcion.
                    return 16;
                }
                else
                {
                    if (aux > 1 || aux2 > 1)
                    {
                        return 33;
                    }
                    else
                    {
                        if (datos[i] == palabrasReemplazo[1])
                        {
                            aux3 = i;
                            aux++;
                        }

                        if(datos[i] == palabrasReemplazo[3])
                        {
                            aux4 = i;
                            aux2++;
                        }
                    }
                }
            }

            if (datos.Contains(palabrasReemplazo[3]))
            {
                /*  ANALIZAR COMO SI EXISTIERA EL WHERE*/

                for(i =0; i < aux3; i++)
                {
                    columnas.Add(datos[i]);
                }

                //Verifica que solo exista un campo despues del from
                if(aux4 - aux3 != 2)
                {
                    return 31;
                }
                
                //Obtiene el nombre de la tabla buscada
                nombreTabla = datos[aux3 + 1];
                
                //Obtiene el filtro
                for(i = aux4 + 1; i < datos.Count; i++)
                {
                    filtro.Add(datos[i]);
                }

                //Verifica la existencia de la tabla
                if (!ExisteTabla(nombreTabla))
                {
                    return 18;
                }

                //Analisis del filtro
                if(filtro.Count > 3 || filtro.Count < 3)
                {
                    return 38;
                }

                if(filtro[0] != "ID")
                {
                    return 36;
                }

                if(filtro[1] != "=")
                {
                    return 38;
                }

                try
                {
                    if(int.Parse(filtro[2]) < 0)
                    {
                        return 39;
                    }
                }
                catch
                {
                    return 39;
                }

                InsertInto infoObtenida = TraerInformacion(nombreTabla);

                //Verificar si se necesita toda la informacion o no "*"
                if (columnas.Count == 1 && columnas[0] == "*")
                {
                    nuevaSeleccion.TableName = nombreTabla;
                    nuevaSeleccion.Columns = infoObtenida.Columns;
                    nuevaSeleccion.Filtro = string.Join("", filtro);
                }
                else
                {
                    //Si se sobrepasa la cantidad de columnas que existen
                    if (columnas.Count > infoObtenida.Columns.Count)
                    {
                        return 35;
                    }

                    //Verica que las columnas concuerden con el formato de tabla almacenado
                    for (i = 0; i < columnas.Count; i++)
                    {
                        if (!infoObtenida.Columns.Contains(columnas[i]))
                        {
                            return 34;
                        }
                    }

                    nuevaSeleccion.TableName = nombreTabla;
                    nuevaSeleccion.Columns = columnas;
                    nuevaSeleccion.Filtro = string.Join("", filtro);
                }
            }
            else
            {
                /*  ANALIZAR SIN WHERE */

                //Obtiene las columnas
                for(i = 0; i < aux3; i++)
                {
                    columnas.Add(datos[i]);
                }

                //Obtiene el nombre 
                if(datos[datos.Count - 2] != palabrasReemplazo[1])
                {
                    return 31;
                }

                nombreTabla = datos[aux3 + 1];

                //Verifica la existencia de la tabla.
                if(!ExisteTabla(nombreTabla))
                {
                    return 18;
                }

                InsertInto infoObtenida = TraerInformacion(nombreTabla);

                //En caso se quiera toda la info...
                if(columnas.Count == 1 && columnas[0] == "*")
                {
                    nuevaSeleccion.TableName = infoObtenida.TableName;
                    nuevaSeleccion.Columns = infoObtenida.Columns;
                }
                else
                {
                    if (columnas.Count > infoObtenida.Columns.Count)
                    {
                        return 35;
                    }

                    //Verica que las columnas concuerden con el formato de tabla almacenado
                    for (i = 0; i < columnas.Count; i++)
                    {
                        if (!infoObtenida.Columns.Contains(columnas[i]))
                        {
                            return 34;
                        }
                    }

                    nuevaSeleccion.TableName = nombreTabla;
                    nuevaSeleccion.Columns = columnas;
                }               
            }

            seleccion = nuevaSeleccion;

            return 0;
        }

        /// <summary>
        /// Verifica la sintaxis cuando se ejecuta el comando Delete
        /// </summary>
        /// <returns>Retorna el numero de error (0 significa que no hay errores) </returns>
        public int VerificiarSintaxisDelete(List<string> datos)
        {
            int i = 0;
            int aux = 1; //Auxiliar para el comando from
            int aux2 = 0;
            int aux3 = 0;

            if(datos[0] != palabrasReemplazo[1])
            {
                return 42;
            }

            datos.RemoveAt(0);

            //Busca algun elemento 'WHERE O FROM' repetido
            //tambien verifica que no se esten usando comandos que no corresponden a la funcion
            for (i = 0; i < datos.Count; i++)
            {
                //Comando Values no debe existir
                if (datos[i] == palabrasReemplazo[7])
                {
                    //Error de comandos que no corresponden a la funcion.
                    return 16;
                }
                else
                {
                    if (aux > 1 || aux2 > 1)
                    {
                        return 33;
                    }
                    else
                    {
                        //From Repetidos
                        if (datos[i] == palabrasReemplazo[1])
                        {
                            aux++;
                        }

                        //Where repetidos
                        if (datos[i] == palabrasReemplazo[3])
                        {
                            aux3 = i;
                            aux2++;
                        }
                    }
                }
            }

            //Dividir los datos
            if(datos.Contains(palabrasReemplazo[3]))
            {
                /*  Existe WHERE    */
                if (aux3 == 1)
                {
                    /*  Verificacion de que exista el archivo    */
                    if(!ExisteTabla(datos[0]))
                    {
                        return 18;
                    }

                    deleteTabla[0] = datos[0];

                    datos.RemoveAt(0); //Quita el nombre
                    datos.RemoveAt(0); //Quita el Where

                    if (datos.Count > 3 || datos.Count < 3)
                    {
                        return 38;
                    }

                    if (datos[0] != "ID")
                    {
                        return 36;
                    }

                    if (datos[1] != "=")
                    {
                        return 38;
                    }

                    try
                    {
                        if (int.Parse(datos[2]) < 0)
                        {
                            return 39;
                        }
                    }
                    catch
                    {
                        return 39;
                    }

                    deleteTabla[1] = string.Join("", datos);
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                /*  NO EXISTE WHERE */
                if(datos.Count != 1)
                {
                    return 1;
                }
                else
                {
                    /*  verificar si exsite el archivo */
                    if(!ExisteTabla(datos[0]))
                    {
                        return 18;
                    }

                    deleteTabla[0] = datos[0];
                    deleteTabla[1] = string.Empty;
                }
            }
            
            return 0;
        }

        /// <summary>
        /// Verifica la sintaxis cuando se ejecuta el comando DropTable.
        /// </summary>
        /// <param name="datos"></param>
        /// <returns>Retorna el numero de error. (0 significa que no hay errores) </returns>
        public int VerificarSintaxisDropTable(List<string> datos)
        {
            if(datos.Count > 1)
            {
                return 40;
            }

            if(datos[0] == palabrasReemplazo[7] || datos[0] == palabrasReemplazo[3] || datos[0] == palabrasReemplazo[1])
            {
                return 41;
            }

            if(!ExisteTabla(datos[0]))
            {
                return 18;
            }

            nombreTabla = datos[0];

            return 0;
        }

        /// <summary>
        /// Verifica la sintaxis cuando se ejecuta el comando Insert Into.
        /// </summary>
        /// <param name="datos"></param>
        /// <returns>Retorna el numero de error. (0 significa que no hay errores) </returns>
        public int VerificarSintaxisInsertTo(List<string> datos)
        {
            int i = 0;
            int aux = 0;
            int aux2 = 0;
            List<string> data = new List<string>();
            List<string> valores = new List<string>();
            
            if(!datos.Contains(palabrasReemplazo[7]))
            {
                //No contiene el comando values
                return 14;
            }

            //Busca algun elemento 'VALUES' repetido
            //tambien verifica que no se esten usando comandos que no corresponden a la funcion
            for(i = 0; i < datos.Count; i++)
            {
                if(datos[i] == palabrasReemplazo[1] || datos[i] == palabrasReemplazo[3])
                {
                    //Error de comandos que no corresponden a la funcion.
                    return 16;
                }
                else 
                {
                    if (aux > 1)
                    {
                        return 15;
                    }
                    else
                    {
                        if (datos[i] == palabrasReemplazo[7])
                        {
                            aux2 = i;
                            aux++;
                        }
                    }
                }                
            }

            //Se divide en 2 partes mi algoritmo

            for(i = 0; i < aux2; i++)
            {
                data.Add(datos[i]);
            }

            for(i = aux2 + 1; i < datos.Count; i++)
            {
                valores.Add(datos[i]);
            }

            /*  VERIFICACION EN LA PARTE DE DATA    */

            //Verificar la existencia de la llave de apertura.
            if (data[1] != "(" && !data.Contains("("))
            {
                //Error de llave de apertura no encontrado
                return 3;
            }

            //En caso que exista algo que no es el operador {
            if (data[1] != "(")
            {
                //Error de espacios de los nombres de las variables
                return 9;
            }

            //En caso el ultimo elemento no es la llave de cierre
            if (data[data.Count - 1] != ")" && !data.Contains(")"))
            {
                return 4;
            }

            //En caso que exista la llave de cierre pero hay mas elementos despues de esto.
            if (data[data.Count - 1] != ")")
            {
                return 1;
            }

            InsertInto nuevaInsercion = new InsertInto();

            nuevaInsercion.TableName = data[0];
            for(i = 2; i < data.Count - 1; i++)
            {
                //Empieza sin tomar en cuenta el nombre y la llave de apertura
                nuevaInsercion.Columns.Add(data[i]);
            }

            /*  VERIFICACIÓN DE LA PARTE VALUES */

            //Verificar la existencia de la llave de apertura.
            if (valores[0] != "(" && !valores.Contains(")"))
            {
                //Error de llave de apertura no encontrado
                return 3;
            }

            //En caso que exista algo que no es el operador {
            if (valores[0] != "(")
            {
                //Error de espacios de los nombres de las variables
                return 9;
            }

            //En caso el ultimo elemento no es la llave de cierre
            if (valores[valores.Count - 1] != ")" && !valores.Contains(")"))
            {
                return 4;
            }

            //En caso que exista la llave de cierre pero hay mas elementos despues de esto.
            if (valores[valores.Count - 1] != ")")
            {
                return 1;
            }

            for(i = 1; i < valores.Count - 1; i++)
            {
                //Omite el operador de apertura y el de cierre
                nuevaInsercion.Values.Add(valores[i]);
            }

            //En caso no concuerden la cantidad de columnas con los valores ingresados.
            if(nuevaInsercion.Values.Count != nuevaInsercion.Columns.Count)
            {
                return 17;
            }

            if(!ExisteTabla(nuevaInsercion.TableName))
            {
                //La tabla que se busca no existe..
                return 18;
            }

            InsertInto infoObtenidaTabla = TraerInformacion(nuevaInsercion.TableName);

            //Verica que las columnas concuerden con el formato de tabla almacenado
            for(i = 0; i < nuevaInsercion.Columns.Count; i++)
            {
                if(nuevaInsercion.Columns[i] != infoObtenidaTabla.Columns[i])
                {
                    return 19;
                }
            }

            for(i = 0; i < infoObtenidaTabla.Types.Count; i++)
            {
                if(infoObtenidaTabla.Types[i] == tiposReservados[1])
                {
                    //Debe ser del tipo Varchar
                    if(!(nuevaInsercion.Values[i][0].ToString() == "'") && !(nuevaInsercion.Values[i][nuevaInsercion.Values[i].Length - 1].ToString() == "'"))
                    {
                        return 20;
                    }

                    //nuevaInsercion.Values[i] = nuevaInsercion.Values[i].Replace("'", string.Empty);
                }

                if(infoObtenidaTabla.Types[i] == tiposReservados[2])
                {
                    //Cuando el tipo de dato es DATETIME
                    if (!(nuevaInsercion.Values[i][0].ToString() == "'") && !(nuevaInsercion.Values[i][nuevaInsercion.Values[i].Length - 1].ToString() == "'"))
                    {
                        return 21;
                    }

                    //nuevaInsercion.Values[i] = nuevaInsercion.Values[i].Replace("'", string.Empty);

                    //string[] probar = nuevaInsercion.Values[i].Split('/');
                    // string[] probar = nuevaInsercion.Values[i].Replace("'", String.Empty).Split('/');

                    string temp = nuevaInsercion.Values[i];
                    string[] probar = temp.Replace("'", string.Empty).Split('/');


                    if(probar.Length != 3 || nuevaInsercion.Values[i].Length != 12)
                    {
                        return 23;
                    }

                    aux = 0;
                    for(int x = 0; x < probar.Length; x++)
                    {
                        try
                        {
                            aux = int.Parse(probar[x]);

                            //Verifica el dia 
                            if(x == 0)
                            {
                                if(aux > 31 || aux < 1)
                                {
                                    return 24;
                                }
                            }

                            //Verifica el mes
                            if(x == 1)
                            {
                                if(aux > 12 || aux < 1)
                                {
                                    return 25;
                                }
                            }

                            //Verifica el año
                            if(x == 2)
                            {
                                if(aux < 1)
                                {
                                    return 26;
                                }
                            }

                        }
                        catch
                        {
                            return 22;
                        }
                    }
                }

                if(infoObtenidaTabla.Types[i] == tiposReservados[3])
                {

                    try
                    {
                        //Cuando el tipo de dato es INT 
                        if (infoObtenidaTabla.Columns[i] == "ID")
                        {
                            if(int.Parse(nuevaInsercion.Values[i]) < 1)
                            {
                                return 29;
                            }
                        }
     
                        int.Parse(nuevaInsercion.Values[i]);
                    }
                    catch
                    {
                        return 22;
                    }
                }
            }

            insertar = nuevaInsercion;

            return 0;
        }

        #endregion

        public Tuple<List<string>, bool> splitArray(string[] complete, int index)
        {
            bool fg = false;
            List<string> newLines = new List<string>();
            for (int i = index; i < complete.Count(); i++)
            {
                if (complete[i].Trim() == ")" || complete[i].Trim() == "}" || complete[i].Trim() == ">" || complete[i].Trim() == "]")
                {
                    fg = true;
                    break;
                }
                newLines.Add(complete[i]);
            }
            return Tuple.Create(newLines, fg);
        }

        public int getSplitIndex(string[] completelns, int startindex, string comando)
        {
            for (int i = startindex; i < completelns.Count(); i++)
            {
                if (completelns[i].Contains(comando))
                    return i + 2;
            }
            return 0;
        }

        public string[] LimiarArray(string[] Lines, string[] toRemove)
        {
            var charsToRemove = toRemove; //eliminar caracteres extra
            for (int i = 0; i < Lines.Length; i++)
            {
                foreach (var c in charsToRemove)
                {
                    Lines[i] = Lines[i].Replace(c, string.Empty);
                }
            }

            Lines = Lines.Where(x => !string.IsNullOrEmpty(x)).ToArray(); // eliminar espacios en blanco

            return Lines;
        }

        #region Insert

        private bool ExisteTabla(string nombre)
        {
            return File.Exists(path + "tablas\\" + nombre + ".tabla");
        }

        private InsertInto TraerInformacion(string nombre)
        {
            StreamReader file = new StreamReader(path + "tablas\\" + nombre + ".tabla");
            InsertInto info = new InsertInto();

            string linea = file.ReadLine();
            string[] elementos = linea.Split(',');
            string[] datos;

            info.TableName = nombre;
            info.Columns.Add(elementos[0]);
            info.Types.Add(tiposReservados[3]);

            for(int i = 1; i < elementos.Length; i++)
            {
                datos = elementos[i].Split(' ');

                info.Columns.Add(datos[0]);
                info.Types.Add(datos[1]);
            }

            file.Close();
            return info;
        }

        public bool insertarArchivoTabla(string tableName, List<string> values)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(path + "tablas\\" + tableName + ".tabla", true))
                {
                    file.WriteLine(string.Join(",", values));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int insertarArbol(string tableName, List<string> values, List<string> columns)
        {
            Fila nuevaFila = new Fila(values);
            BTree<int, Fila> tree = new BTree<int, Fila>(tableName, path + "\\arbolesb");

            if (tree.Buscar(nuevaFila.Id) == int.MinValue)
            {
                tree.Insertar(nuevaFila.Id, nuevaFila);
                tree.Cerrar();
            }
            else
            {
                return 46;
            }

            return 0;
        }

        public int Insertar(InsertInto insercion)
        {
            return insertarArbol(insercion.TableName.Trim(), insercion.Values, insercion.Columns);           
        }

        #endregion

        #region SELECT
        public List<string> listDataTable = new List<string>();
        public List<string> Missing = new List<string>();

        //public bool Select(string[] columns, string tableName, int index)
        public int Select(Selection seleccion)
        {
            try
            {
                int i = 0;
                int j = 0;
                int x = 0;

                listDataTable = new List<string>();
                BTree<int, Fila> tree = new BTree<int, Fila>(seleccion.TableName, path + "\\arbolesb");  // cargar arbol               
                List<string> showlst = new List<string>(); //Tabla para mostrar
                string dataObtenida = string.Empty;

                List<string> temp;
                List<string> columnas;

                #region Select

                if(seleccion.Filtro != string.Empty)
                {
                    //Caso en el que existe un filtro de ID
                    string[] filtro = seleccion.Filtro.Split('=');

                    dataObtenida = tree.TraerData(int.Parse(filtro[1]));

                    if(dataObtenida == string.Empty)
                    {
                        return 45;
                    }


                    dataObtenida = dataObtenida.Replace("#", "");
                    temp = dataObtenida.Split('_').ToList();

                    //Elimino el espacio en blanco del final
                    if(temp[temp.Count - 1] == string.Empty)
                    {
                        temp.RemoveAt(temp.Count - 1);
                    }
                    
                    columnas = TraerColumnas(seleccion.TableName);

                    //Ingresar Cabecera

                    string auxiliar = string.Empty;

                    for(i = 0; i < seleccion.Columns.Count; i++)
                    {
                        auxiliar += seleccion.Columns[i] + ",";
                    }

                    showlst.Add(auxiliar.TrimEnd(','));

                    //Agregar datos
                    auxiliar = string.Empty;
                    for(i = 0; i < columnas.Count; i++)
                    {
                        for(j = 0; j < seleccion.Columns.Count; j++)
                        {
                            if (columnas[i] == seleccion.Columns[j])
                            {
                                auxiliar += temp[i] + ",";
                                break;
                            }
                        }
                    }

                    showlst.Add(auxiliar.TrimEnd(','));
                }
                else
                {
                    //Obtengo los id's del arbol 
                    List<string> IdLst = tree.RecorrerArbol();
                    IdLst.Sort();

                    //Ingresar Cabecera
                    string auxiliar = string.Empty;

                    for (i = 0; i < seleccion.Columns.Count; i++)
                    {
                        auxiliar += seleccion.Columns[i] + ",";
                    }

                    showlst.Add(auxiliar.TrimEnd(','));

                    columnas = TraerColumnas(seleccion.TableName);

                    for (i = 0; i < IdLst.Count; i++)
                    {
                        auxiliar = string.Empty;
                        dataObtenida = tree.TraerData(int.Parse(IdLst[i]));
                        dataObtenida = dataObtenida.Replace("#", "");
                        temp = dataObtenida.Split('_').ToList();

                        //Elimino el espacio en blanco del final
                        if (temp[temp.Count - 1] == string.Empty)
                        {
                            temp.RemoveAt(temp.Count - 1);
                        }
                   
                        //Agregar datos
                        auxiliar = string.Empty;
                        for (x = 0; x < columnas.Count; x++)
                        {
                            for (j = 0; j < seleccion.Columns.Count; j++)
                            {
                                if (columnas[x] == seleccion.Columns[j])
                                {
                                    auxiliar += temp[x] + ",";
                                    break;
                                }
                            }
                        }

                        showlst.Add(auxiliar.TrimEnd(','));
                    }
                }
                             
                #endregion 
       
                listDataTable = showlst;

                return 0;
            }
            catch
            {
                return 44;
            }
        }

        private List<string> TraerColumnas(string nombreTabla)
        {
            StreamReader file = new StreamReader(path + "tablas\\" + nombreTabla + ".tabla");
            List<string> columnas = new List<string>();

            string linea = file.ReadLine();
            string[] elementos = linea.Split(',');
            string[] datos;

            columnas.Add(elementos[0]);

            for (int i = 1; i < elementos.Length; i++)
            {
                datos = elementos[i].Split(' ');
                columnas.Add(datos[0]);
            }
            file.Close();

            return columnas;
        }

        #endregion

        //****Sustituir "DELETE FROM" por su comando****
        //****Sustituir "WHERE" por su comando****
        #region DELETE
        public int DeleteFrom(string[] datos)
        {
            try
            {
                listDataTable = new List<string>();
                //BTree<int, Fila> tree = new BTree<int, Fila>(seleccion.TableName, path + "\\arbolesb");  // cargar arbol               
                //List<string> showlst = new List<string>(); //Tabla para mostrar
                string dataObtenida = string.Empty;
                
                if(datos[1] != string.Empty)
                {
                    return 43;
                }
                else
                {
                    /*  SE ANALIZA SIN WHERE    */

                    //Elimino todo el arbol
                    File.Delete(path + "arbolesb\\" + datos[0]);

                    //Creo nuevamente el árbol pero inicia vacio
                    BTree<int, Fila> tree = new BTree<int, Fila>(datos[0], 5, path + "\\arbolesb");
                    return 0;
                }
            }
            catch
            {
                return 44;
            }
        }

        #endregion

        #region DROP TABLE
        public bool DropTable(string tableName)
        {
            try
            {
                listDataTable = new List<string>();        
                File.Delete(path + "tablas\\" + tableName + ".tabla");
                File.Delete(path + "arbolesb\\" + tableName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion


        public void MostrarContenidoArbol(string NombreTabla)
        {
            int i = 0;

            listDataTable = new List<string>();
            BTree<int, Fila> tree = new BTree<int, Fila>(NombreTabla, path + "\\arbolesb");  // cargar arbol     
            List<string> columnas = new List<string>();                     
            List<string> showlst = new List<string>(); //Tabla para mostrar

            string dataObtenida = string.Empty;

            List<string> IdLst = tree.RecorrerArbol();          
            IdLst.Sort();

            columnas = TraerColumnas(NombreTabla);

            //Ingresar Cabecera
            string auxiliar = string.Empty;

            for (i = 0; i < columnas.Count; i++)
            {
                auxiliar += columnas[i] + ",";
            }
         
            showlst.Add(auxiliar.TrimEnd(','));

            for (i = 0; i < IdLst.Count; i++)
            {
                auxiliar = string.Empty;
                dataObtenida = tree.TraerData(int.Parse(IdLst[i]));
                dataObtenida = dataObtenida.Replace("#", "");
                dataObtenida = dataObtenida.Replace("_", ",");

                showlst.Add(dataObtenida.TrimEnd(','));
            }

            listDataTable = showlst;
        }

        //****Sustituir la palabara "UPDATE" por su comando****
        //****Sustituir "WHERE" por su comando****
        #region UPDATE 
        public bool Update(string[] Lines)
        {
            try
            {
                string tableName = Lines[Array.IndexOf(Lines, "UPDATE") + 1]; //obtener nombre de la tabla
                listDataTable = new List<string>();
                string data = File.ReadAllText(path + "tablas\\" + tableName + ".tabla").Replace("\r\n", "$"); //cargar tabla

                //****Arreglar asunto con el arbol primero*****
               /* BTree<int, standardObject> tree = new BTree<int, standardObject>(tableName, 5); */// cargar arbol

                string[] Table = data.Split('$');
                bool[] fgcolumns = new bool[9]; //campos a modificar
                string key = "";


                for (int k = 0; k < Lines.Count(); k++) //obtener llave para modificar
                {
                    if (Lines[k].Trim() == palabrasReemplazo[0])
                    {
                        key = Lines[k + 1].Replace("ID =", string.Empty);
                        break;
                    }
                }

                for (int i = 0; i < Table.Count(); i++) //buscar la llave 
                {
                    string[] row = Table[i].Split(',');
                    if (row[0].Trim() == key.Trim()) //modificar
                    {



                        break;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        /*
         * UPDATE 
         * < nombre de la tabla> 
         * SET 
         * < nombre de la columna > = < valor > 
         * WHERE  
         * ID = < valor > 
         * */
        #endregion
    }
}

