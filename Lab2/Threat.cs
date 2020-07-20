using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    /// <summary>
    /// Угроза безопасности информации.
    /// </summary>
    public class Threat
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string ImpactObject { get; set; }
        public bool Privacy { get; set; }
        public bool Integrity { get; set; }
        public bool Availability { get; set; }
        public Threat (string id, string name, string description, string source, string impactObject, string privacy, string integrity, string availability)
        {
            ID = "УБИ." + id;
            Name = name;
            Description = description;
            Source = source;
            ImpactObject = impactObject;
            /*if (privacy == 1)
            {
                Privacy = true;
            }
            else
            {
                Privacy = false;
            }
            if (integrity == 1)
            {
                Integrity = true;
            }
            else
            {
                Integrity = false;
            }
            if (availability == 1)
            {
                Availability = true;
            }
            else
            {
                Availability = false;
            }*/
            /*Privacy = privacy;
            Integrity = integrity;
            Availability = availability;*/
            if (privacy == "True" || privacy == "1")
            {
                Privacy = true;
            }
            else
            {
                Privacy = false;
            }
            if (integrity == "True" || integrity == "1")
            {
                Integrity = true;
            }
            else
            {
                Integrity = false;
            }
            if (availability == "True" || availability == "1")
            {
                Availability = true;
            }
            else
            {
                Availability = false;
            }
            
        }
        public override string ToString()
        {
            return $"ID: {ID}\n\nНаименование угрозы: {Name}\n\nОписание угрозы: {Description}\n\nИсточник угрозы: {Source}\n\nОбъект воздействия: {ImpactObject}\n\nНарушение конфиденциальности: {Privacy}\n\nНарушение целостности: {Integrity}\n\nНарушение доступности: {Availability}";
            //return $"{ID}♪{Name}♪{Description}♪{Source}♪{ImpactObject}♪{Privacy}♪{Integrity}♪{Availability}";
        }
    }
}
