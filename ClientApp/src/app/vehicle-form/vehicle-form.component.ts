import * as _ from 'underscore';
import { VehicleService } from './../services/vehicle.service';
import { Component, OnInit } from '@angular/core';
import 'rxjs/add/observable/forkJoin';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastsManager } from 'ng2-toastr';
import { Observable } from 'rxjs/Observable';
import { Vehicle, SaveVehicle } from '../models/vehicle';

@Component({
  selector: 'vehicle-form',
  templateUrl: './vehicle-form.component.html',
  styleUrls: ['./vehicle-form.component.css']
})
export class VehicleFormComponent implements OnInit {

  makes: any[];
  models: any[];
  features: any[];
  vehicle: SaveVehicle = {
    id: 0,
    makeId: 0,
    modelId: 0,
    isRegistered: false,
    features: [],
    contact: {
      name: '',
      email: '',
      phone: '',
    }
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private vehicleService: VehicleService,
    private toastr: ToastsManager,
 )
  {
    route.params.subscribe(p => {
      this.vehicle.id = +p['id'] || 0;
    });
  }

  ngOnInit() {
    var sources = [
      this.vehicleService.getMakes(),
      this.vehicleService.getFeatures(),
    ];

    if (this.vehicle.id) {
      sources.push(this.vehicleService.getVehicle(this.vehicle.id));
    }

    Observable.forkJoin(sources).subscribe(data => {
      this.makes = data[0];
      this.features = data[1];

      if (this.vehicle.id) {
        this.setVehicle(data[2]);
        this.populateModels();
      }
    }, err => {

      if (err.status == 404)
        this.router.navigate(['/home']);
      });

  }

  private setVehicle(v: Vehicle) {
    this.vehicle.id = v.id;
    this.vehicle.makeId = v.make.id;
    this.vehicle.modelId = v.model.id;
    this.vehicle.isRegistered = v.isRegistered;
    this.vehicle.contact = v.contact;
    this.vehicle.features = _.pluck(v.features, 'id');
  }

  private populateModels() {
    var selectedMake = this.makes.find(m => m.id == this.vehicle.makeId);
    this.models = selectedMake ? selectedMake.models : [];
  }

  onMakeChange(): void {
    //re-populate model list
    var selectedMake = this.makes.find(make => make.id == this.vehicle.makeId);
    this.models = selectedMake ? selectedMake.models : [];

    delete this.vehicle.modelId;
  }

  onFeatureToggle(featureId, $event): void {
    if ($event.target.checked) {
      this.vehicle.features.push(featureId);
    }
    else {
      var index = this.vehicle.features.indexOf(featureId);
      this.vehicle.features.splice(index, 1);
    }
  }

  submit() {
    var result$ = (this.vehicle.id) ? this.vehicleService.update(this.vehicle) : this.vehicleService.create(this.vehicle);
    result$.subscribe(vehicle => {
      this.toastr.success("Vehicle saved successfully", "Success");
      this.router.navigate(['/vehicles/', vehicle.id])
    });
  }

  delete(): void {
    if (confirm('Do you really want to delete this vehicle?')) {
      this.vehicleService.delete(this.vehicle.id)
        .subscribe(x => {
          this.router.navigate(['/home']);
        })
    }
  }

}
