import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})

export class MainComponent implements OnInit {

  //featurecode: string|null = 'bs18';

  constructor(private route: ActivatedRoute)
  {
  }

  ngOnInit(): void 
  {
    /*
    ngOnInit只會再建立component時執行一次而已，不用subscribe的話，會娶不到routerlink的參數
    ref：https://stackoverflow.com/questions/55822548/angular-routerlink-does-not-trigger-ngoninit
    */
    //this.route.params.subscribe((params: any) => {
    //  this.featurecode = params.featurecode;
    //});  
  }
}

