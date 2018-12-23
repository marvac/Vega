import * as _ from 'underscore';
import { VehicleService } from './../services/vehicle.service';
import { Component, OnInit } from '@angular/core';
import 'rxjs/add/Observable/forkJoin';
import { Router } from '@angular/router';

@Component({
  selector: 'vehicle-form',
  templateUrl: './vehicle-form.component.html',
  styleUrls: ['./vehicle-form.component.css']


})
export class VehicleFormComponent implements OnInit {

  makes: any[];
  models: any[];
  features: any[];
  vehicle: any = {
    features: [],
    contact: {}
  };

  constructor(private router: Router, private vehicleService: VehicleService) { }

  ngOnInit() {
    this.vehicleService.getMakes().subscribe(makes => {
      this.makes = makes;
    })

    this.vehicleService.getFeatures().subscribe(features => this.features = features);
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

  submit(): void {

    var result = this.vehicleService.create(this.vehicle);

    result.subscribe(vehicle => console.log(vehicle),
      error => {
        //todo: swap this with a toast module
        console.log(`Error: ${error}`)
      });
  }

}
