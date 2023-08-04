import { LoadingInfo } from './../../shared/components/loading/loading.component';
import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

/*
*子元件透過ShareService傳遞資料給父元件
*https://stackoverflow.com/questions/37662456/angular-2-output-from-router-outlet
*/
export class ShareService {

  // Observable string sources
  private emitChangeSource = new Subject<any>();
  // Observable string streams
  changeEmitted$ = this.emitChangeSource.asObservable();

  // Service message commands
  emitChange(change: any) 
  {
      this.emitChangeSource.next(change);
  }
}
