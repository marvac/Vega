import { VehicleService } from './../services/vehicle.service';
import { Component, OnInit, ElementRef, ViewChild, NgZone } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastsManager } from 'ng2-toastr';
import { PhotoService } from '../services/photo.service';
import { ProgressService } from '../services/progress.service';

@Component({
  templateUrl: './view-vehicle.component.html',
  styles: [
    '[hidden] { display: none !important; }'
  ]
})
export class ViewVehicleComponent implements OnInit {
  @ViewChild('fileInput') fileInput: ElementRef;
  vehicle: any;
  vehicleId: number;
  photos: any[];
  progress: any;

  constructor(
    private zone: NgZone,
    private route: ActivatedRoute,
    private router: Router,
    private vehicleService: VehicleService,
    private toastr: ToastsManager,
    private progressService: ProgressService,
    private photoService: PhotoService) {

    route.params.subscribe(p => {
      this.vehicleId = +p['id'];
      if (isNaN(this.vehicleId) || this.vehicleId <= 0) {
        router.navigate(['/vehicles']);
        return;
      }
    });
  }

  ngOnInit() {
    this.photoService.getPhotos(this.vehicleId)
      .subscribe(p => this.photos = p);

    this.vehicleService.getVehicle(this.vehicleId)
      .subscribe(
        v => this.vehicle = v,
        err => {
          if (err.status == 404) {
            this.router.navigate(['/vehicles']);
            return;
          }
        });
  }

  delete() {
    if (confirm("Are you sure?")) {
      this.vehicleService.delete(this.vehicle.id)
        .subscribe(x => {
          this.router.navigate(['/vehicles']);
        });
    }
  }

  uploadPhoto() {
    var nativeElement: HTMLInputElement = this.fileInput.nativeElement;

    this.progressService.startTracking()
      .subscribe(progress => {
        console.log(progress)
        this.zone.run(() => {
          this.progress = progress
        })
      },
        null, //no need to handle errors here
        () => {
          this.progress = null;
        });

    this.photoService.upload(this.vehicleId, nativeElement.files[0])
      .subscribe(photo => {

        this.photos.push(photo);
        this.toastr.success(photo.fileName, "Success");
      });
  }
}
