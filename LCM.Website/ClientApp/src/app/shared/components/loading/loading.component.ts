import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-loading',
  templateUrl: './loading.component.html',
  styleUrls: ['./loading.component.scss']
})
export class LoadingComponent {

  @Input() loader: boolean = false;  
  @Input() loadingMsg: string = "處理中...";

  constructor() { }

  ngOnInit(): void {
    
  }

}
