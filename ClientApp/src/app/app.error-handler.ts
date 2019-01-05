import { ErrorHandler, NgZone, Injectable, Injector } from "@angular/core";
import { ToastsManager } from "ng2-toastr";

@Injectable()
export class AppErrorHandler implements ErrorHandler {

  private toastr: ToastsManager;

  constructor(private injector:Injector,  private ngZone: NgZone) { }

  handleError(error: any): void {
    this.toastr = this.injector.get(ToastsManager);

    this.ngZone.run(() => {
      this.toastr.error(error._body, "Error");
    });

    }

}
