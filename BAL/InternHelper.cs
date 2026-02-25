using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using InternApp.Models;
using Npgsql;

namespace InternApp.BAL
{
    public class InternHelper
    {
        private readonly NpgsqlConnection _conn;
        public InternHelper(NpgsqlConnection connection)
        {
            _conn = connection;
        }

        public List<t_Topics> GetTopics()
        {
            List<t_Topics> topics = new List<t_Topics>();

            try
            {
                using var cmd = new NpgsqlCommand("SELECT c_topicid, c_topic_name FROM t_topicnames", _conn);

                _conn.Open();
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    topics.Add(new t_Topics
                    {
                        c_TopicId = Convert.ToInt32(reader["c_topicid"]),
                        c_TopicName = reader["c_topic_name"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _conn.Close();
            }

            return topics;
        }


        public string AddIntern(t_Intern intern)
        {
            try
            {
                using var cmd = new NpgsqlCommand(@"INSERT INTO t_interns (c_internname, c_gender, c_topicid, c_date_of_presentation,c_status, c_topic_image) VALUES (@internname, @gender, @topicid, @dateofpresented, @status, @topicimage);", _conn);

                cmd.Parameters.AddWithValue("internname", intern.c_InternName);
                cmd.Parameters.AddWithValue("gender", intern.c_Gender);
                cmd.Parameters.AddWithValue("topicid", intern.c_TopicId);
                cmd.Parameters.AddWithValue("dateofpresented", intern.c_PresentationDate);
                cmd.Parameters.AddWithValue("status", intern.c_IsPresented);
                cmd.Parameters.AddWithValue("topicimage", intern.ImagePath ?? (object)DBNull.Value);

                _conn.Open();
                cmd.ExecuteNonQuery();

                return "Intern Added Successfully";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                _conn.Close();
            }
        }


        public List<t_Intern> GetAllInterns()
        {
            List<t_Intern> interns = new List<t_Intern>();

            try
            {
                using var cmd = new NpgsqlCommand(@"
            SELECT i.c_internid,
                   i.c_internname,
                   i.c_gender,
                   i.c_topicid,
                   t.c_topic_name,
                   i.c_date_of_presentation,
                   i.c_status,
                   i.c_topic_image
            FROM t_interns i
            JOIN t_topicnames t ON i.c_topicid = t.c_topicid
            ORDER BY i.c_internid ASC", _conn);

                _conn.Open();
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    interns.Add(new t_Intern
                    {
                        c_InternId = Convert.ToInt32(reader["c_internid"]),
                        c_InternName = reader["c_internname"].ToString(),
                        c_Gender = reader["c_gender"].ToString(),
                        c_TopicName = reader["c_topic_name"].ToString(),
                        c_PresentationDate = reader.GetFieldValue<DateOnly>("c_date_of_presentation").ToDateTime(TimeOnly.MinValue),
                        c_IsPresented = Convert.ToBoolean(reader["c_status"]),
                        ImagePath = reader["c_topic_image"].ToString()
                    });
                }
            }
            finally
            {
                _conn.Close();
            }

            return interns;
        }


        public string DeleteIntern(int id)
        {
            try
            {
                using var cmd = new NpgsqlCommand("DELETE FROM t_interns WHERE c_internid = @id", _conn);
                cmd.Parameters.AddWithValue("@id", id);
                _conn.Open();

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? "Intern deleted successfully" : "Failed to delete intern";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                _conn.Close();
            }
        }


        public t_Intern GetInternById(int id)
        {
            t_Intern model = null;
            try
            {
                var qry = @"Select i.c_internid,i.c_internname,i.c_gender,t.c_topic_name,t.c_topicid,i.c_date_of_presentation,i.c_status,i.c_topic_image
                From t_interns i
                Inner Join t_topicnames t
                On i.c_topicid = t.c_topicid
                Where i.c_internid=@id;";
                _conn.Open();

                using (var cmd = new NpgsqlCommand(qry, _conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model = new t_Intern
                            {
                                c_InternId = reader.GetInt32(0),
                                c_InternName = reader.GetString(1),
                                c_Gender = reader.GetString(2),
                                c_TopicName = reader.GetString(3),
                                c_TopicId = reader.GetInt32(4),
                                c_PresentationDate = reader.GetDateTime(5),
                                c_IsPresented = reader.GetBoolean(6),
                                ImagePath = reader.IsDBNull(7) ? null : reader.GetString(7)
                            };
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                _conn.Close();
            }
            return model;
        }


        public t_Intern FetchInternDetails(int id)
        {
            var intern = new t_Intern();

            _conn.Open();

            using var command = new NpgsqlCommand(
                @"SELECT i.*, t.c_topic_name 
          FROM t_interns i
          LEFT JOIN t_topicnames t ON i.c_topicid = t.c_topicid
          WHERE i.c_internid = @Id",
                _conn
            );

            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                intern.c_InternId = Convert.ToInt32(reader["c_internid"]);
                intern.c_InternName = reader["c_internname"].ToString();
                intern.c_Gender = reader["c_gender"].ToString();
                intern.c_TopicId = Convert.ToInt32(reader["c_topicid"]);
                intern.c_TopicName = reader["c_topic_name"].ToString();
                intern.c_PresentationDate = reader["c_date_of_presentation"] == DBNull.Value
                            ? (DateTime?)null
                            : Convert.ToDateTime(reader["c_date_of_presentation"]);
                intern.c_IsPresented = Convert.ToBoolean(reader["c_status"]);

                intern.ImagePath = reader["c_topic_image"] == DBNull.Value
                    ? null
                    : reader["c_topic_image"].ToString();
            }

            _conn.Close();

            return intern;
        }



        public bool UpdateIntern(t_Intern intern)
        {
            _conn.Open();

            string query = @"UPDATE t_interns
                     SET c_internname = @name,
                         c_gender = @gender,
                         c_topicid = @c_TopicId,
                         c_date_of_presentation = @date,
                         c_status = @status,
                         c_topic_image = @image
                     WHERE c_internid = @id";

            using var cmd = new NpgsqlCommand(query, _conn);

            cmd.Parameters.AddWithValue("@name", intern.c_InternName);
            cmd.Parameters.AddWithValue("@gender", intern.c_Gender);
            cmd.Parameters.AddWithValue("@c_TopicId", intern.c_TopicId);
            cmd.Parameters.AddWithValue("@date", intern.c_PresentationDate);
            cmd.Parameters.AddWithValue("@status", intern.c_IsPresented);
            cmd.Parameters.AddWithValue("@image", intern.ImagePath);
            cmd.Parameters.AddWithValue("@id", intern.c_InternId);

            int result = cmd.ExecuteNonQuery();

            _conn.Close();

            return result > 0;
        }


    }
}