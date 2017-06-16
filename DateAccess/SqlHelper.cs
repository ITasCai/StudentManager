using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace DataAccess
{
    /// <summary>
    /// 静态帮助类
    /// </summary>
    public static class SqlHelper
    {
        /// <summary>
        /// 设置连接字符串
        /// </summary>
        private static readonly string strConn = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;

        /// <summary>
        /// 声明一个SqlTransaction对象
        /// </summary>
        private static SqlTransaction tran;

        /// <summary>
        /// 声明一个SqlConnection对象
        /// </summary>
        private static SqlConnection conn;

        /// <summary>
        /// 返回连接对象
        /// </summary>
        public static SqlConnection Conn
        {
            get
            {
                if (conn == null)
                {
                    conn = new SqlConnection(strConn);
                }
                return conn;
            }
        }

        #region 执行增删改 ExecuteNonQuery
        /// <summary>
        /// 执行增删改
        /// </summary>
        /// <param name="cmdType">命令类型(Text或者Proc)</param>
        /// <param name="strSql">命令文本(SQL语句或者存储过程名称)</param>
        /// <param name="param">传递的参数</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(CommandType cmdType, string strSql, params SqlParameter[] param)
        {
            SqlConnection con = new SqlConnection(strConn);//创建一个连接对象，用来指定连接的数据源
            try
            {
                SqlCommand cmd = new SqlCommand(strSql, con);//创建一个命令对象，用来执行SQL语句
                cmd.CommandType = cmdType;//设置命令类型
                if (param != null)
                {
                    cmd.Parameters.AddRange(param);//添加参数
                }
                con.Open();//最晚打开，最早关闭
                int count = cmd.ExecuteNonQuery();
                return count;//返回受影响的行数
            }
            catch (Exception ex)
            {
                //con.Close();
                //throw;
                Console.WriteLine(ex.Message);
                return -1;
            }
            finally
            {
                con.Close();
            }
        }
        #endregion

        #region 返回单个值 ExecuteScalar<T>
        /// <summary>
        /// 返回单个值(第一行第一列)
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="strSql">SQL语句</param>
        /// <param name="param">传递的参数</param>
        /// <returns>返回自定义类型结果</returns>
        public static T ExecuteScalar<T>(CommandType cmdType, string strSql, params SqlParameter[] param)
        {
            SqlConnection con = new SqlConnection(strConn);
            try
            {
                SqlCommand cmd = new SqlCommand(strSql, con);
                cmd.CommandType = cmdType;
                if (param != null)
                {
                    cmd.Parameters.AddRange(param);
                }
                con.Open();
                object result = cmd.ExecuteScalar();//返回单个结果
                if (result != null)
                {
                    return (T)Convert.ChangeType(result, typeof(T));//typeof(T)
                }
                return default(T);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default(T);
            }
            finally
            {
                con.Close();
            }
        }
        #endregion

        #region 查询多条数据 ExecuteReader
        /// <summary>
        /// 查询多条数据(多行多列)
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="strSql">SQL语句</param>
        /// <param name="param">传递的参数</param>
        /// <returns>返回SqlDataReader对象</returns>
        public static SqlDataReader ExecuteReader(CommandType cmdType, string strSql, params SqlParameter[] param)
        {
            SqlConnection con = new SqlConnection(strConn);
            try
            {
                SqlCommand cmd = new SqlCommand(strSql, con);
                cmd.CommandType = cmdType;
                if (param != null)
                {
                    cmd.Parameters.AddRange(param);
                }
                con.Open();
                //CommandBehavior.CloseConnection 当读取完dr的数据时,执行dr.Close()后,则dr所依赖的所有对象，都会关闭。
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return dr;
            }
            catch (Exception ex)
            {
                con.Close();
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion

        #region 查询结果集 DataSet和DataTable

        /// <summary>
        /// 查询结果集
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="strSql">SQL语句</param>
        /// <param name="param">传递的参数</param>
        /// <returns>返回DataSet</returns>
        public static DataSet ExecuteDataSet(CommandType cmdType, string strSql, params SqlParameter[] param)
        {
            SqlConnection con = new SqlConnection(strConn);
            try
            {
                SqlCommand cmd = new SqlCommand(strSql, con);
                cmd.CommandType = cmdType;
                if (param != null)
                {
                    cmd.Parameters.AddRange(param);
                }
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);//创建一个数据适配器对象
                DataSet ds = new DataSet();//创建一个DataSet的实例，用于存放数据
                da.Fill(ds);//填充数据集
                return ds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// 查询DataTable
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="strSql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(CommandType cmdType, string strSql, params SqlParameter[] param)
        {
            DataSet ds = ExecuteDataSet(cmdType, strSql, param);//返回DataSet
            return ds.Tables[0];
        }
        #endregion

        #region 执行事务
        /// <summary>
        /// 执行事务操作
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="strSql">SQL语句</param>
        /// <param name="param">传递的参数</param>
        public static void ExecuteTransaction(CommandType cmdType, string strSql, params SqlParameter[] param)
        {
            SqlCommand cmd = new SqlCommand(strSql, conn);
            cmd.CommandType = cmdType;
            if (param != null)
            {
                cmd.Parameters.AddRange(param);//添加参数
            }
            if (tran != null)
            {
                cmd.Transaction = tran;//设置事务
            }
            cmd.ExecuteNonQuery();
        }


        /// <summary>
        /// 开启事务
        /// </summary>
        public static void BeginTransaction()
        {
            Conn.Open();//打开连接
            tran = conn.BeginTransaction();//开启事务
        }


        /// <summary>
        /// 提交事务
        /// </summary>
        public static void CommitTransaction()
        {
            tran.Commit();//提交事务
            Conn.Close();//关闭连接
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public static void RollBackTransaction()
        {
            tran.Rollback();//回滚事务
            Conn.Close();//关闭连接
        }
        #endregion

    }
}
