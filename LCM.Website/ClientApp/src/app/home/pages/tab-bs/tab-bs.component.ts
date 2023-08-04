import { ShareService } from './../../../core/service/share.service';
import { AfterViewInit, Component } from '@angular/core';

@Component({
  selector: 'app-tab-bs',
  templateUrl: './tab-bs.component.html',
  styleUrls: ['./tab-bs.component.scss']
})
export class TabBsComponent implements AfterViewInit {

  constructor
  (
    private _shareService: ShareService
  ){}

  ngAfterViewInit()
  {
    setTimeout(() => {
      this._shareService.emitChange("大小18報表比對/手動結案作業(Tab Ver.)");
    });   
  }

}
