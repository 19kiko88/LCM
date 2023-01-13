import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {Router} from '@angular/router';
import {NavItem} from '../../models/nav-item';

/**
 * Ref：Dynamic Nested Menus https://stackblitz.com/edit/dynamic-nested-menus-yhg6kz?file=app%2Fapp.component.html
 */
@Component({
  selector: 'app-menu-item',
  templateUrl: './menu-item.component.html',
  styleUrls: ['./menu-item.component.scss']
})
export class MenuItemComponent {
  @Input()items!: NavItem[];
  @ViewChild('childMenu') public childMenu: any;

  constructor(public router: Router) {
  }

  ngOnInit() {
  }
}
