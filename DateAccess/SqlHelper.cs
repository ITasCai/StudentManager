using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace DateAccess
{
    /// <summary>
    /// 静态帮助类
    /// </summary>
     public static class SqlHelper
    {
        /// <summary>
        /// 设置连接字符串
        /// </summary>
        private static readonly string strConn= ConfigurationManager.ConnectionStrings["ConnStr"].ToString();

        /// <summary>
        /// 声明了一个SqlTransaction对象
        /// </summary>
        private static SqlTransaction tran;

        /// <summary>
        /// 声明了一个SqlConnection对象
        /// </summary>
        private static SqlConnection conn;

        /// <summary>
        /// 返回连接对象
        /// </summary>
        public static SqlConnection Conn {
            get
            {
                if (conn == null)
                {
                    //创建一个连接对象
                    conn = new SqlConnection(strConn);

                }
                return conn;
            }
        }

        #region 执行增删改
        /// <summary>
        /// 执行增删改
        /// </summary>
        /// <param name="cmdType">命令类型（Text或者Proc）</param>
        /// <param name="strSql">命令文本（SQL语句或者存储过程名称）</param>
        /// <param name="param">传递的参数</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(CommandType cmdType, string strSql, params SqlParameter[] param)
        {

            //创建连接对象，打开数据源
            SqlConnection con = new SqlConnection(strConn);
            try
            {

                //创建命令对象，用来执行sql语句
                SqlCommand cmd = new SqlCommand(strSql, con);
                //设置命令类型
                cmd.CommandType = cmdType;

                //如果参数不为空，添加参数
                if (param != null)
                {
                    //添加参数
                    cmd.Parameters.AddRange(param);
                }
                //打开连接(最晚打开，最早关闭)
                con.Open();

                int count = cmd.ExecuteNonQuery();


                //返回受影响的行数
                return count;
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


        #region 返回单个值（第一行第一列）
        /// <summary>
        /// 返回单个值（第一行第一列）
        /// </summary>
        /// <param name="cmdType">命令类型（Text或者Proc）</param>
        /// <param name="strSql">SQL语句</param>
        /// <param name="param">传递的参数</param>
        /// <returns>返回自定义类型结果</returns>
        public static T ExecuteScalar<T>(CommandType cmdType, string strSql, params SqlParameter[] param)
        {

            //创建连接对象，打开数据源
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
                //返回单个结果
                object result = cmd.ExecuteScalar();

                T t = default(T);
                t = (T)Convert.ChangeType(result, typeof(object));
                return t;
            }
            catch (Exception ex)
            {

                return default(T);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                con.Close();
            }

        }

        #endregion


        #region 返回多条数据（多行多列）

        /// <summary>
        /// 返回多条数据（多行多列）
        /// </summary>
        /// <param name="cmdType">命令类型（Text或者Proc）</param>
        /// <param name="strSql">sql语句</param>
        /// <param name="param">传递的参数</param>
        /// <returns></returns>
        public static SqlDataReader ExedcuteReader(CommandType cmdType, string strSql, params SqlParameter[] param)
        {

            //创建连接对象，打开数据源
            SqlConnection con = new SqlConnection(strConn);

            try
            {
                //创建命令对象，用来执行sql语句
                SqlCommand cmd = new SqlCommand(strSql, con);
                //设置命令类型
                cmd.CommandType = cmdType;

                if (param != null)
                {
                    cmd.Parameters.AddRange(param);

                }
                con.Open();
                //CommandBehavior.CloseConnection:当读取完dr的数据时，执行dr.Close()后，则 dr所依赖对的所有对象，都会关闭
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


        #region 返回查询结果集
        /// <summary>
        /// 返回查询结果集
        /// </summary>
        /// <param name="cmdType">命令类型（Text或者Proc）</param>
        /// <param name="strSql">SQL语句</param>
        /// <param name="param">传递的参数</param>
        /// <returns>返回DataSet</returns>
        public static DataSet ExecuteDataSet(CommandType cmdType, string strSql, params SqlParameter[] param)
        {

            SqlConnection con = new SqlConnection(strConn);

            try
            {
                //创建命令对象，用来执行sql语句
                SqlCommand cmd = new SqlCommand(strSql, con);
                //设置命令类型
                cmd.CommandType = cmdType;

                if (param != null)
                {
                    cmd.Parameters.AddRange(param);

                }
                con.Open();
                //创建一个数据适配器对象
                SqlDataAdapter da = new SqlDataAdapter();
                //创建一个DataSet实例，用于存放数据
                DataSet ds = new DataSet();
                //填充数据集
                da.Fill(ds);

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
        #endregion




        /// <summary>
        /// 查询DataTable
        /// </summary>
        /// <param name="cmdType">命令类型（Text或者Proc）</param>
        /// <param name="strSql">SQL语句</param>
        /// <param name="param">传递的参数</param>
        /// <returns>返回DataSet</returns>

        public static DataTable ExecuteDataTable(CommandType cmdType, string strSql, params SqlParameter[] param) {

            DataSet ds = ExecuteDataSet(cmdType,strSql,param);
            return ds.Tables[0];

        }



        #region 执行事务
        /// <summary>
        /// 执行事务操作
        /// </summary>
        /// <param name="cmdType">命令类型（Text或者Proc）</param>
        /// <param name="strSql">SQL语句</param>
        /// <param name="param">传递的参数</param>
        /// <returns>返回DataSet</returns>
        public static void ExecuteTransaction(CommandType cmdType, string strSql, params SqlParameter[] param)
        {

            SqlCommand cmd = new SqlCommand(strSql, conn);
            cmd.CommandType = cmdType;
            if (param != null)
            {
                //添加参数
                cmd.Parameters.AddRange(param);
            }
            if (tran != null)
            {
                //设置事务
                cmd.Transaction = tran;

            }
            cmd.ExecuteNonQuery();
        }

        public static void BeginTransaction()
        {
            //打开连接
            Conn.Open();
            //开启事务
            tran = conn.BeginTransaction();
        }

        public static void CommitTransaction()
        {
            //提交事务
            tran.Commit();
            //关闭连接
            Conn.Close();
        }

        public static void RollBackTransaction()
        {

            //回滚事务
            tran.Rollback();
            Conn.Close();
        }
        #endregion
    }
}
