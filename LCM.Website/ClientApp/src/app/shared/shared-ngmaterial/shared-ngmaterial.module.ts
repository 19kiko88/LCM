import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button'
import {MatMenuModule} from '@angular/material/menu';
import {MatIconModule} from '@angular/material/icon';
import { MaterialTabsComponent } from './material-tabs/material-tabs.component'
import { MatTabsModule } from '@angular/material/tabs'



@NgModule({
  declarations: [
    MaterialTabsComponent
  ],
  imports: [
    CommonModule,
    MatTabsModule
  ],
  exports: [
    MatButtonModule,
    MatMenuModule,
    MatIconModule,
    MatTabsModule,
    MaterialTabsComponent
  ]
})
export class SharedNgMaterialModule { }
