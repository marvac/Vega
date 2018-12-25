import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';

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
    return this.http.get(`/api/vehicles/${id}`)
      .map(response => response.json());
  }

  create(vehicle) {
    return this.http.post(this.vehiclesEndpoint, vehicle)
      .map(res => res.json());
  }
  
}
