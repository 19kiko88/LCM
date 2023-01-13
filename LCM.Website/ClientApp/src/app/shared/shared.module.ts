import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedNgMaterialModule } from './shared-ngmaterial/shared-ngmaterial.module';
import { MenuItemComponent } from './components/menu-item/menu-item.component'
import { RouterModule } from '@angular/router';
import { SharedNgbootstrapModule } from './shared-ngbootstrap/shared-ngbootstrap.module';
import { UploadComponent } from './components/upload/upload.component';

@NgModule({
  declarations: [MenuItemComponent, UploadComponent],
  imports: [
    CommonModule,
    SharedNgMaterialModule,
    SharedNgbootstrapModule,
    RouterModule
  ],
  exports:[MenuItemComponent, UploadComponent]
})
export class SharedModule { }
