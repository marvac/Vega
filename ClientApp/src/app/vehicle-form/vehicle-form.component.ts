import * as _ from 'underscore';
import { VehicleService } from './../services/vehicle.service';
import { Component, OnInit, ViewContainerRef } from '@angular/core';
import 'rxjs/add/Observable/forkJoin';
import { Router } from '@angular/router';
import { ToastsManager } from 'ng2-toastr';

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

  constructor(private router: Router, private vehicleService: VehicleService, private toastr: ToastsManager, vcr: ViewContainerRef)
  {
    this.toastr.setRootViewContainerRef(vcr);
  }

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

    result.subscribe(vehicle => {
      this.toastr.success('Submitted successfully', 'Success');
    });
  }

}
