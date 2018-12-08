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
  vehicle: any = {};

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

  submit(): void {

    var result = this.vehicleService.create(this.vehicle);

    result.subscribe(vehicle => {
      //this.toastService.success({
      //  title: 'Success',
      //  msg: 'Data was sucessfully saved.',
      //  theme: 'bootstrap',
      //  showClose: true,
      //  timeout: 5000
      //});

      this.router.navigate(['/vehicles/', vehicle.id])
    })
  }

}
