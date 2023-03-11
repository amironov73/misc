import {Plugin} from 'obsidian';

export default class NeatPlugin extends Plugin {

  async onload() {
    this.registerMarkdownCodeBlockProcessor('neat', (source, el) => {
      const rows = source.split("\n").filter(row => row.length > 0);
      if (rows.length > 0) {

        let width = '300'
        if (rows.length > 1) {
          width = rows[1]
        }

        const activeFile = this.app.workspace.getActiveFile()!
        if (activeFile) {
          let activePath = this.app.vault.getResourcePath(activeFile)
          activePath = activePath.substring(0, activePath.lastIndexOf("/"))
          const fileName = activePath + '/' + rows[0]
          const div = el.createDiv({
            attr: {'style': 'clear: both;'}
          })
          div.createEl("img", {
            attr:
              {
                'width': width,
                'style': 'float: right; margin: 4mm;',
                'src': fileName
              }
          })
        }
      }
    })
  }

  onunload() {
    // пустое тело метода
  }

}

