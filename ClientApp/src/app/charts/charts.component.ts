import { Component } from '@angular/core';

@Component({
  templateUrl: './charts.component.html'
})
export class ChartsComponent {
  data = {
    labels: ['BMW', 'Audi', 'Mazda'],
    datasets: [
      {
        data: [5, 3, 1],
        backgroundColor: [
          "#ff6384",
          "#36a2eb",
          "#ffce56"
        ]
      }
    ]
  }

  constructor() {

  }

  
}
