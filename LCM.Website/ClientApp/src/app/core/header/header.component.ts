import { ChangeDetectorRef, Component, AfterViewInit } from '@angular/core';
import {NavItem} from '../../shared/models/nav-item';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements AfterViewInit {

  constructor(
    private cd: ChangeDetectorRef
    ){}

  navItems: NavItem[] = [
    {
      displayName: '功能清單',
      iconName: 'cloud_upload',
      route: 'bs188',
      children: [
        {
          displayName: '大小18比對',
          iconName: 'chevron_right',  
          route: 'bs18',
        },
        {
          displayName: '手動結案',
          iconName: 'chevron_right',  
          route: 'manual_close',
        },
        {
          displayName: '大小18比對/手動結案(tab ver.)',
          iconName: 'chevron_right',  
          route: 'tab',
        }
      ]
    }
    // ,{
    //   displayName: '測試第一階功能',
    //   iconName: 'close',
    //   route: 'main',
    //   children: [
    //     {
    //       displayName: 'Speakers',
    //       iconName: 'group',  
    //       route: 'main',
    //     }
    //   ]
    // }
  ]

  ngAfterViewInit(): void 
  {
    /*
    *解決 NG0100: ExpressionChangedAfterItHasBeenCheckedError
    *Ref：https://juejin.cn/post/6844903582555176973
    */
    this.cd.detectChanges();
  }
}

