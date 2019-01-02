import { VehicleService } from "./services/vehicle.service";
import { Component, OnInit } from "@angular/core";
import { KeyValuePair, Vehicle } from "./models/vehicle";

@Component({
  templateUrl: 'vehicle-list.component.html'
})
export class VehicleListComponent implements OnInit {

  vehicles: Vehicle[];
  makes: KeyValuePair[];
  query: any = {};
  columns = [
    {title: 'Id'},
    {title: 'Contact Name', key: 'contactName', isSortable: true},
    { title: 'Make', key: 'make', isSortable: true},
    { title: 'Model', key: 'model', isSortable: true},
    {} //view vehicle link here
  ];

  constructor(private vehicleService: VehicleService) { }

  ngOnInit() {

    this.vehicleService.getMakes()
      .subscribe(makes => this.makes = makes);

    this.populateVehicles();

  }

  private populateVehicles() {
    this.vehicleService.getVehicles(this.query)
      .subscribe(v => this.vehicles = v);
  }

  onFilterChange() {

    this.populateVehicles();
  }

  resetFilter() {
    this.query = {};
    this.onFilterChange();
  }

  sortBy(columnName: string) {
    if (this.query.sortBy === columnName) {
      this.query.isSortAscending = !this.query.isSortAscending;
    }
    else {
      this.query.sortBy = columnName;
      this.query.isSortAscending = true;
    }

    this.populateVehicles();
  }

}
