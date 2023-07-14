import {Component} from '@angular/core';
//import {VERSION} from '@angular/material';
import {NavItem} from '../../shared/models/nav-item';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  //version = VERSION;

  navItems: NavItem[] = [
    {
      displayName: '功能清單',
      iconName: 'cloud_upload',
      route: 'bs188',
      children: [
        {
          displayName: '大小18比對',
          iconName: 'label_important',  
          route: 'bs18',
        },
        {
          displayName: '手動結案',
          iconName: 'label_important',  
          route: 'manual_close',
        },
        {
          displayName: '大小18比對/手動結案(tab ver.)',
          iconName: 'label_important',  
          route: 'main',
        }
      ]
    },
    // {
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
}

