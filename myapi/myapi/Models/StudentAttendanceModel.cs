using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace myapi.Models
{
    public class StudentAttendanceModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string? StudentId { get; set; }
        [BsonElement("studentname")]
        public string StudentName { get; set; }
        [BsonElement("class")]
        public string ClassName { get; set; }
        [BsonElement("section")]
        public string SectionName { get; set; }
        [BsonElement("attendance_date")]
        public string AttendanceDate { get; set; }
        [BsonElement("is_present")]
        public bool IsPresent { get; set; }
        [BsonElement("remarks")]
        public string Remarks { get; set; }
    }
}
