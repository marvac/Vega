import { ErrorHandler } from "@angular/core";
import { ToastsManager } from "ng2-toastr";

export class AppErrorHandler implements ErrorHandler {

  constructor(private toastr: ToastsManager) { }

  handleError(error: any): void {
    console.log("error");
    }

}
