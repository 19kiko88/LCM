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
      displayName: '大小18報表處理',
      iconName: 'cloud_upload',
      route: 'bs18',
      children: [
        // {
        //   displayName: '大小18比對',
        //   iconName: 'cloud_upload',  
        //   route: 'bs18',
        // }
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

