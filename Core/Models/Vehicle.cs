﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vega.Core.Models
{
    [Table("Vehicles")]
    public class Vehicle
    {
        public int Id { get; set; }

        public int ModelId { get; set; }
        public Model Model { get; set; }

        public bool IsRegistered { get; set; }
        [Required, StringLength(255)]

        public string ContactName { get; set; }

        [Required, StringLength(255)]
        public string ContactPhone { get; set; }

        public string ContactEmail { get; set; }

        public DateTime LastUpdated { get; set; }

        public ICollection<VehicleFeature> Features { get; set; } = new Collection<VehicleFeature>();

        public ICollection<Photo> Photos { get; set; } = new Collection<Photo>();
    }
}
