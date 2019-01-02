import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';
import { SaveVehicle } from '../models/vehicle';

@Injectable()
export class VehicleService {

  private readonly vehiclesEndpoint = '/api/vehicles';

  constructor(private http: Http) { }

  getMakes() {
    return this.http.get('/api/makes')
      .map(response => response.json());
  }

  getFeatures() {
    return this.http.get('/api/features')
      .map(response => response.json());
  }

  getVehicle(id) {
    return this.http.get(`${this.vehiclesEndpoint}/${id}`)
      .map(response => response.json());
  }

  getVehicles(query) {
    return this.http.get(`${this.vehiclesEndpoint}?${this.toQueryString(query)}`)
      .map(response => response.json());
  }

  toQueryString(obj) {
    var parts = [];
    for (var property in obj) {

      var value = obj[property];
      if (value != null && value != undefined) {
        parts.push(`${encodeURIComponent(property)}=${encodeURIComponent(value)}`);
      }
    }

    return parts.join('&');
  }

  create(vehicle) {
    return this.http.post(this.vehiclesEndpoint, vehicle)
      .map(res => res.json());
  }

  update(vehicle: SaveVehicle) {
    return this.http.put(`${this.vehiclesEndpoint}/${vehicle.id}`, vehicle)
      .map(res => res.json());
  }

  delete(id) {
    return this.http.delete(`${this.vehiclesEndpoint}/${id}`)
      .map(response => response.json());
  }
  
}
