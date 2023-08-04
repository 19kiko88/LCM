import { LoadingService } from './shared/components/loading/loading.service';
import { ShareService } from './core/service/share.service';
import { Component, OnInit } from '@angular/core';
import { FALSE } from 'sass';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

export class AppComponent implements OnInit
{
  title: string = '';
  isLoader: boolean = false;
  loadingMsg: string|undefined = '';

  constructor(
    private _shareService: ShareService,
    private _loadingService: LoadingService
  ){}

  ngOnInit(): void 
  {
    //Setting title
    this._shareService.changeEmitted$.subscribe(res => {
      this.title = res;
    })

    //Setting Loading
    this._loadingService.loader$.subscribe(res => {
      this.isLoader = res.isLoading;
      this.loadingMsg = res.loadingMessage;
    })

  }  

}
