using GeometriaModels.DALs;
using GeometriaModels.DALs.Utilities;
using GeometriaModels.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometriaMSQLDALsImpl
{
    public class FigurasMSQLDAL : FigurasDal<SqlTransaction>
    {
        private readonly string _connectionString;

        public FigurasMSQLDAL(string connectionStrings)
        {
            _connectionString = connectionStrings;
        }
        public FigurasMSQLDAL(IOptions<ConnectionString> options)
        {
            _connectionString = options.Value.DefaultConnection;
        }

        async public Task<List<FiguraModel>> GetAll(ITransactionDAL<SqlTransaction>? transaction = null)
        {
            List<FiguraModel> figuras = new List<FiguraModel>();

            #region conexion y comando sql
            string query = @"
SELECT f.Id,
	   f.Tipo,
	   f.Area,
	   f.Ancho,
	   f.Largo,
	   f.Radio
FROM Figuras f
ORDER BY f.Id
";

            var conn = await GetOpenedConnectionAsync(transaction);

            using SqlCommand command = new SqlCommand(query, conn, transaction?.GetInternalTransaction());

            using SqlDataReader dataReader = await command.ExecuteReaderAsync();
            #endregion

            while (await dataReader.ReadAsync())
            {
                figuras.Add(this.ReadAsObjeto(dataReader));
            }

            return figuras;
        }

        async public Task<FiguraModel?> GetByKey(int idFigura, ITransactionDAL<SqlTransaction>? transaccion = null)
        {
            FiguraModel? figura = null;

            string query = @"
SELECT TOP 1    f.Id,
	           f.Tipo AS Tipo,
	           f.Area,
	           f.Ancho,
	           f.Largo,
	           f.Radio
FROM Figuras f
WHERE f.Id=@Id
ORDER BY f.Area
";
            SqlConnection conn = await GetOpenedConnectionAsync(transaccion);

            #region comando sql
            using SqlCommand cmd = new SqlCommand(query, conn, transaccion?.GetInternalTransaction());
            cmd.Parameters.AddWithValue("@Id", idFigura);
            #endregion

            using SqlDataReader dataReader = await cmd.ExecuteReaderAsync();

            if (await dataReader.ReadAsync())
            {
                figura = this.ReadAsObjeto(dataReader);
            }

            return figura;
        }

        async public Task<FiguraModel?> Add(FiguraModel nuevo, ITransactionDAL<SqlTransaction>? transaccion = null)
        {
            int tipo = 0;
            double? ancho = null;
            double? largo = null;
            double? radio = null;

            string query = @"
INSERT INTO Figuras (Tipo, Ancho, Largo, Radio)
OUTPUT INSERTED.Id
VALUES
(@Tipo, @Ancho, @Largo, @Radio)
";

            SqlConnection conn = await GetOpenedConnectionAsync(transaccion);

            using SqlCommand comm = new SqlCommand(query, conn, transaccion?.GetInternalTransaction());

            if (nuevo is RectanguloModel r)
            {
                tipo = 1;
                ancho = r.Ancho;
                largo = r.Largo;
            }
            else if (nuevo is CirculoModel c)
            {
                tipo = 2;
                radio = c.Radio;
            }

            comm.Parameters.AddWithValue("@Tipo", tipo);
            comm.Parameters.AddWithValue("@Ancho", ancho ?? (object)DBNull.Value);
            comm.Parameters.AddWithValue("@Largo", largo ?? (object)DBNull.Value);
            comm.Parameters.AddWithValue("@Radio", radio ?? (object)DBNull.Value);

            object idObject = await comm.ExecuteScalarAsync();

            nuevo.Id = Convert.ToInt32(idObject);

            return nuevo;
        }

        async public Task<bool> Save(FiguraModel entidad, ITransactionDAL<SqlTransaction>? transaccion = null)
        {
            int id = 0;
            double? area = null;
            double? ancho = null;
            double? largo = null;
            double? radio = null;

            string query = @"
UPDATE Figuras SET Area=@Area, Ancho=@Ancho, Largo=@Largo, Radio=@Radio
WHERE Id=@Id_Figura
";
            var conn = await GetOpenedConnectionAsync(transaccion);

            using SqlCommand cmd = new SqlCommand(query, conn, transaccion?.GetInternalTransaction());

            id = entidad.Id ?? 0;
            if (entidad is RectanguloModel r)
            {
                ancho = r.Ancho;
                largo = r.Largo;
            }
            else if (entidad is CirculoModel c)
            {
                radio = c.Radio;
            }
            area = entidad.Area;

            cmd.Parameters.AddWithValue("@Id_Figura", id);
            cmd.Parameters.AddWithValue("@Area", area ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Ancho", ancho ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Largo", largo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Radio", radio ?? (object)DBNull.Value);

            int cantidad = await cmd.ExecuteNonQueryAsync();

            return id > cantidad;
        }

        async public Task<bool> Remove(int idFigura, ITransactionDAL<SqlTransaction>? transaccion = null)
        {
            string query = @"
DELETE 
FROM Figuras 
WHERE Id=@Id_Figura
";

            SqlConnection conn = await GetOpenedConnectionAsync(transaccion);

            #region sqlcommand
            using SqlCommand cmd = new SqlCommand(query, conn, transaccion?.GetInternalTransaction());
            cmd.Parameters.AddWithValue("@Id_Figura", idFigura);
            #endregion

            int cantidad = await cmd.ExecuteNonQueryAsync();

            return cantidad > 0;
        }

        private async Task<SqlConnection> GetOpenedConnectionAsync(ITransactionDAL<SqlTransaction>? transaccion)
        {
            var conexion = transaccion?.GetInternalTransaction()?.Connection ?? new SqlConnection(_connectionString);

            if (conexion.State == System.Data.ConnectionState.Closed)
            {
                await conexion.OpenAsync();
            }
            return conexion;
        }

        async public Task ProcesarFiguras(ITransactionDAL<SqlTransaction>? transaccion = null)
        {
            string query = "sp_CalcularAreas";
            var conn = await GetOpenedConnectionAsync(transaccion);

            using SqlCommand cmd = new SqlCommand(query, conn, transaccion?.GetInternalTransaction());
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            _ = await cmd.ExecuteNonQueryAsync();

        }

        private FiguraModel ReadAsObjeto(SqlDataReader dataReader)
        {
            #region parseo
            int? id = dataReader["Id"] != DBNull.Value ? Convert.ToInt32(dataReader["Id"]) : null;
            int? tipo = dataReader["Tipo"] != DBNull.Value ? Convert.ToInt32(dataReader["Tipo"]) : null;
            double? area = Convert.ToInt32(dataReader["Area"] != DBNull.Value ? dataReader["Area"] : null);
            double? ancho = Convert.ToInt32(dataReader["Ancho"] != DBNull.Value ? dataReader["Ancho"] : null);
            double? largo = Convert.ToInt32(dataReader["Largo"] != DBNull.Value ? dataReader["Largo"] : null);
            double? radio = Convert.ToInt32(dataReader["Radio"] != DBNull.Value ? dataReader["Radio"] : null);
            #endregion

            FiguraModel entidad = null;
            if (tipo == 1)
            {
                entidad = new RectanguloModel() { Id = id, Area = area, Ancho = ancho, Largo = largo };
            }
            else if (tipo == 2)
            {
                entidad = new CirculoModel() { Id = id, Area = area, Radio = radio };
            }
            return entidad;
        }

    }
}
